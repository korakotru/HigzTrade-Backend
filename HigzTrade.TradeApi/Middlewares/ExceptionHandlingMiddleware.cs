using Hangfire;
using HigzTrade.Domain.Exceptions;
using HigzTrade.Infrastructure.BackgroundJobs;
using HigzTrade.Infrastructure.ExternalServices;
using HigzTrade.TradeApi.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Extensions.Hosting;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace HigzTrade.TradeApi.Middlewares
{
    // เหตุผลที่ควรแยก ExceptionHandlerMiddleware ออกมา เพราะจะ customize ได้มากกว่า app.UseExceptionHandler เช่น การ inject MailService เพื่อให้สามารถส่งเมลล์ให้ support เมื่อเกิด error 500 เป็นต้น
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _log;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> log,
            IHostEnvironment env)
        {
            _next = next;
            _log = log;
            _env = env;
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
                return;//throw;
            }

            var (title, status) = ExceptionHelper.GetErrorInfo(ex);
            string errorDetail = ex.Message; 

            if (status >= 500)
            {
                if (_env.IsProduction()) { errorDetail = "Internal error, please contact customer support."; } // ซ่อนข้อความจริงและส่งเมลแจ้งเตือน(ถ้าเป็น production)

                // Send email included critical error detail to application support
                BackgroundJob.Enqueue<SendApplicationErrorEmailJob>(job =>
                       job.ExecuteAsync(
                           ctx.TraceIdentifier,
                           status,
                           title,
                           ex.Message,
                           ex.StackTrace ?? string.Empty));

                //_ = Task.Run(async () =>
                //{
                //    try
                //    {
                //        await mailService.SendApplicationErrorEmailAsync(ctx.TraceIdentifier, status, title, ex.Message, ex.StackTrace ?? "");
                //    }
                //    catch (Exception mailEx)
                //    {
                //        // ต้องมี try-catch ครอบเสมอ! 
                //        // เพราะถ้าพังตรงนี้แล้วไม่มีใครจับ มันจะทำให้ Process หลักมีปัญหา
                //        Log.Error(mailEx, "Failed to send error email in background");
                //    }
                //});
            }


            // Structured logging → ทำงานร่วมกับ Serilog / Seq ได้
            //_log.LogError(ex, "{ErrorTitle} at {Path} (Status={Status})", title, ctx.Request.Path, status); // ให้ Serilog ทำการ log ทีเดียว
             


            ctx.Response.Clear();
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Title = title,
                Status = status,
                Detail = errorDetail,
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
