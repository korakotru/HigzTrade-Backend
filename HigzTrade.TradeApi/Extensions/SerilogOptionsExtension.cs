using HigzTrade.TradeApi.Helpers;
using Microsoft.AspNetCore.Diagnostics;

namespace HigzTrade.TradeApi.Extensions
{
    public static class SerilogOptionsExtension
    {
        public static void ConfigCustomOptions(this Serilog.AspNetCore.RequestLoggingOptions options)
        {
            // 1. ปรับ Message Template ให้มีข้อมูลที่เราต้องการครบในบรรทัดเดียว
            //options.MessageTemplate = "{Title} at {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                // ข้อมูล Client
                //diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                //diagnosticContext.Set("UserId", httpContext.User?.Identity?.Name ?? "Anonymous");

                //// ดึง Exception จาก Features (ที่ถูก Set มาจาก Middleware ตัวก่อนหน้า)
                //var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

                //if (exception != null)
                //{
                //    var errorInfo = ExceptionHelper.GetErrorInfo(exception);

                //    diagnosticContext.Set("ErrorTitle", errorInfo.Title); // ตรงนี้จะเปลี่ยนตามประเภท Exception จริง
                //    diagnosticContext.Set("IsError", true);
                //    diagnosticContext.Set("Detail", exception.Message);
                //}
                //else
                //{
                //    diagnosticContext.Set("ErrorTitle", "Success");
                //    diagnosticContext.Set("IsError", false);
                //}

                // ใส่ Build Version ไปด้วยเพื่อความตึงในการตรวจสอบ
                //diagnosticContext.Set("BuildVersion", AppVersionHelpers.GetBuildVersion());
            };
        }
    }
}
