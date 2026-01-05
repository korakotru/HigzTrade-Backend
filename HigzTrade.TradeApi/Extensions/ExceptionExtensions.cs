using Hangfire;
using HigzTrade.Infrastructure.BackgroundJobs;
using HigzTrade.TradeApi.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using System.Text.Json;

namespace HigzTrade.TradeApi.Extensions
{
    public static class ExceptionExtensions
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async ctx =>
                { 

                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
                    var diagnosticContext = ctx.RequestServices.GetRequiredService<IDiagnosticContext>();

                    var feature = ctx.Features.Get<IExceptionHandlerFeature>();
                    var ex = feature?.Error;


                    if (ctx.Response.HasStarted)
                    {
                        logger.LogWarning(ex, "Response already started, cannot write error response");
                        return;
                    }

                    if (ex == null)
                    {
                        return;
                    }


                    var (title, status) = ExceptionHelper.GetErrorInfo(ex);

                    diagnosticContext.Set("ClientIP", ctx.Connection.RemoteIpAddress?.ToString());
                    diagnosticContext.Set("UserId", ctx.User?.Identity?.Name ?? "Anonymous");
                    diagnosticContext.Set("Title", status >= 500 ? "Error" : "Success");  
                    diagnosticContext.Set("IsError", status >= 500);
                    diagnosticContext.Set("Detail", ex.Message);
                    diagnosticContext.Set("BuildVersion", AppVersionHelpers.GetBuildVersion());

                    // ถ้าเป็น Critical Error (500+) หรือ DB พัง
                    if (status >= 500)
                    {
                        // ส่งเมลผ่าน Hangfire แบบ Async (Fire and Forget ไม่รอ response)
                        BackgroundJob.Enqueue<SendApplicationErrorEmailJob>(job =>
                            job.ExecuteAsync(
                                ctx.TraceIdentifier,
                                status,
                                title,
                                ex.Message,
                                ex.StackTrace ?? string.Empty));
                    } 

                    ctx.Response.Clear();
                    ctx.Response.StatusCode = status;
                    ctx.Response.ContentType = "application/problem+json";

                    var problem = new ProblemDetails
                    {
                        Title = title,
                        Status = status,
                        Detail = env.IsProduction() ? "Internal error, please contact customer support.": ex.Message,
                        Instance = ctx.Request.Path
                    };

                    problem.Extensions["traceId"] = ctx.TraceIdentifier; // ใช้หา error ใน log และส่งไปให้ thrid-party เป็น traceId เดียวกัน
                    problem.Extensions["build"] = AppVersionHelpers.GetBuildVersion(); // เพื่อให้ client/ui เช็ค version api จาก build ได้

                    if (env.IsDevelopment())
                    {
                        problem.Extensions["stackTrace"] = ex.StackTrace;
                    }

                    var json = JsonSerializer.Serialize(
                        problem,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    await ctx.Response.WriteAsync(json);
                });
            });
        }
    }
}
