using Microsoft.AspNetCore.Http.Timeouts;

namespace HigzTrade.TradeApi.Extensions
{
    public static class RequestTimeoutExtension
    {
        public static IServiceCollection AddCustomRequestTimeouts(this IServiceCollection services)
        {
            services.AddRequestTimeouts(options => {
                // ตั้งค่าเริ่มต้นให้ทุก Request ต้องจบภายใน 15 วินาที (ป้องกัน Slowloris Attack และป้องกัน thread ไปค้างที่ request บางตัวที่ช้าผิดปกติ )
                options.DefaultPolicy = new RequestTimeoutPolicy
                {
                    Timeout = TimeSpan.FromSeconds(15),
                    TimeoutStatusCode = 504
                };

                // สำหรับ process ที่ต้องประมวลผลนานกว่าปกติ
                //options.AddPolicy(RequestTimeoutCustomPolicy.BigDataProcessingPolicy, new RequestTimeoutPolicy
                //{
                //    Timeout = TimeSpan.FromMinutes(2),
                //    TimeoutStatusCode = 504
                //});
            });

            return services;
        }
    }
}
