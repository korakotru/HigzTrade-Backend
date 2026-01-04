using HigzTrade.Domain.Exceptions;
using HigzTrade.TradeApi.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;
using System.Text.Json;
using HigzTrade.Infrastructure.ExternalServices;

namespace HigzTrade.TradeApi.Middlewares
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _log;
        private readonly IHostEnvironment _env;
        private readonly MailService _mailService;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> log,
            IHostEnvironment env,
            MailService mailService)
        {
            _next = next;
            _log = log;
            _env = env;
            _mailService = mailService;//todo:korakot - send email to support, when error 500 occurred.
        }

        // Method ชื่อ Invoke หรือ InvokeAsync: Framework จะมองหา Method ที่มีชื่อเป๊ะๆ แบบนี้ และต้องรับ HttpContext เป็น Parameter ตัวแรกเสมอ
        public async Task InvokeAsync(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (OperationCanceledException)
            {
                // request ถูก cancel → ไม่ต้อง log เป็น error
                ctx.Response.StatusCode = 499; // Client Closed Request
            }
            catch (Exception ex)
            {
                await ExceptionHandleAsync(ctx, ex);
            }
        }

        private async Task ExceptionHandleAsync(HttpContext ctx, Exception ex)
        {
            if (ctx.Response.HasStarted)
            {
                _log.LogWarning(ex, "Response already started, cannot write error response");
                //throw;
                return;
            }

            var (title, status) = ExceptionHelper.GetErrorInfo(ex);


            // Structured logging → ทำงานร่วมกับ Serilog / Seq ได้
            //_log.LogError(ex, "{ErrorTitle} at {Path} (Status={Status})", title, ctx.Request.Path, status); // ให้ Serilog ทำการ log ทีเดียว

            // เก็บ Exception ไว้ใน Feature เพื่อให้ Serilog (DiagnosticContext) มองเห็น
            ctx.Features.Set<IExceptionHandlerFeature>(new ExceptionHandlerFeature { Error = ex });

            ctx.Response.Clear();
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Title = title,
                Status = status,
                Detail = (ex is DbUpdateException) ? ex.Message : "Internal error, please contact customer support.",
                Instance = ctx.Request.Path
            };

            // แสดง stack trace เฉพาะ DEV
            problem.Extensions["traceId"] = ctx.TraceIdentifier;
            if (_env.IsDevelopment())
            {
                problem.Extensions["stackTrace"] = ex.StackTrace;
            }
            problem.Extensions["build"] = AppVersionHelpers.GetBuildVersion();

            var json = JsonSerializer.Serialize(
                problem,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await ctx.Response.WriteAsync(json);
        }
    }
}
