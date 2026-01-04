//using HigzTrade.Application.Interfaces;
using HigzTrade.Infrastructure.Persistence.Context;
using HigzTrade.Infrastructure.Persistence.Repositories;
using HigzTrade.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HigzTrade.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // แยกการ Register DbContext มาไว้ที่นี่
            services.AddDbContext<HigzTradeDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("Database"); 
                options.UseSqlServer(connectionString);
                options.UseSnakeCaseNamingConvention(); // ตัวแปลงชื่อใน database จาก snake_case เป็น PascalCase อัตโนมัติ
                options.UseLazyLoadingProxies(false); // Turn off EF LazyLoad
            });

            // Register Repository
            services.AddScoped<EfUnitOfWork, EfUnitOfWork>();

            services.Scan(scan => scan
                .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName != null && a.FullName.Contains("HigzTrade.Infrastructure")))
                .AddClasses(classes => classes
                    .Where(type => type.Namespace != null &&
                                   type.Namespace.StartsWith("HigzTrade.Infrastructure.Persistence.Repositories") &&
                                   (type.Name.EndsWith("Repository") || type.Name.EndsWith("Query")))
                )
                //.AsImplementedInterfaces() // Dependency Injection (inject to interface)
                .AsSelf() // Dependency Injection (direct injection to self class)
                .WithScopedLifetime());


            return services;
        }
    }
}
