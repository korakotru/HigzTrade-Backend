using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName != null && a.FullName.Contains("HigzTrade.Application"))) // สแกนจาก "หมุด" ในเลเยอร์ Application
                .AddClasses(classes => classes
                    .Where(type => type.Namespace != null &&
                                   type.Namespace.StartsWith("HigzTrade.Application.UseCases") &&
                                   type.Name.EndsWith("UseCase")) // กรองเอาเฉพาะคลาสที่ลงท้ายด้วย UseCase
                )
                .AsSelf() // Direct Injection จดทะเบียนเป็น Class ตัวมันเอง (เพื่อให้ฉีด CreateProductUseCase ได้โดยตรง)
                .WithScopedLifetime()); // กำหนดอายุขัยตาม Request

            return services;
        }
    }
}
