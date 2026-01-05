using Hangfire;
using HigzTrade.Domain.Exceptions;
using HigzTrade.Infrastructure.BackgroundJobs;
using HigzTrade.TradeApi.Helpers;
using HigzTrade.TradeApi.HttpResponseModels;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
                await _next(ctx); // โค้ด try-catch ครอบการเรียก _next(context) *** เพราะเราออกแบบ class นีให้เป็น ExceptionHandlingMiddleware 
            }
            catch (OperationCanceledException)
            {
                // request ถูก cancel ไม่ต้อง log เป็น error
                ctx.Response.StatusCode = 499; // Client Closed Request
            }
            catch (BusinessException ex)
            {
                await ExceptionHandleAsync(ctx, ex);
            }
            catch (DbUpdateException ex)
            {
                await ExceptionHandleAsync(ctx, ex.GetBaseException());
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
                _log.LogWarning(
                    ex,
                    "Response already started, cannot write error response");
                return;
            }

            (string title, int status) = ExceptionHelper.GetErrorInfo(ex);
            var diagnosticContext = ctx.RequestServices.GetRequiredService<IDiagnosticContext>();
            diagnosticContext.Set("Title", status >= 500 ? "Error" : "Success");
            diagnosticContext.Set("IsError", status >= 500);
            diagnosticContext.Set("Detail", ex.Message);
            diagnosticContext.Set("ClientIP", ctx.Connection.RemoteIpAddress?.ToString());
            diagnosticContext.Set("UserId", ctx.User?.Identity?.Name ?? "Anonymous");
            diagnosticContext.Set("BuildVersion", AppVersionHelpers.GetBuildVersion());
            diagnosticContext.Set("StackTrace", status == 400 ? ex.StackTrace?.Substring(0, 500) ?? "" : ex.StackTrace);


            string[] errors;
            // ===== Side effect เฉพาะ Server Error =====
            if (status >= 500)
            {
                errors = new[] { _env.IsProduction() ?  "Error occurred, please contact support team." : ex.Message };

                BackgroundJob.Enqueue<SendApplicationErrorEmailJob>(job =>
                    job.ExecuteAsync(
                        ctx.TraceIdentifier,
                        status,
                        title,
                        ex.Message,
                        ex.StackTrace ?? string.Empty));
            }
            else if (ex is BusinessException bizEx)
            {
                errors = bizEx.Errors.ToArray();
            }
            else
            {
                errors = new[] { ex.Message };
            }

            ctx.Response.Clear();
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/json";

            var response = new ApiErrorResponse
            {
                Title = title,
                Status = status,
                Errors = errors,
                TraceId = ctx.TraceIdentifier
            };

            await ctx.Response.WriteAsJsonAsync(response);
        }
    }
}
