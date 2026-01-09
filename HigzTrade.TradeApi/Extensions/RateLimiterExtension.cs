using HigzTrade.TradeApi.Constants;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace HigzTrade.TradeApi.Extensions
{
    public static class RateLimiterExtension
    {
        public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(RateLimitPolicy.DefaultRateLimit, opt => //(fixed,sliding,token,concurrency)
                {
                    opt.PermitLimit = 100;              // สูงสุด ? request
                    opt.Window = TimeSpan.FromMinutes(1); // ใน ? นาที
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;// OldestFirst,NewestFirst  (default = OldestFirst)
                    opt.QueueLimit = 50;                // 50 queue ถ้าเกิน ratelimit
                });

                options.AddSlidingWindowLimiter(RateLimitPolicy.AuthRateLimit, opt => //(fixed,sliding,token,concurrency)
                {
                    opt.PermitLimit = 1;              // สูงสุด ? request
                    opt.Window = TimeSpan.FromSeconds(1); // ใน ? วินาที
                    opt.SegmentsPerWindow = 4; // opt.Window / 4
                });

                options.RejectionStatusCode = 429; // Too Many Requests
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json";

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too Many Requests",
                        message = "You have exceeded the rate limit. Please try again later."
                    }, cancellationToken: token);
                };
            });

            return services;// return for allow chaining call 

        }
    }
}
