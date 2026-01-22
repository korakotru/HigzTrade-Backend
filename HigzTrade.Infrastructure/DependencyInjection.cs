//using HigzTrade.Application.Interfaces;
using Hangfire;
using HigzTrade.Application.Interfaces;
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
            var connectionString = configuration.GetConnectionString("Database");

            // แยกการ Register DbContext มาไว้ที่นี่
            services.AddDbContextPool<HigzTradeDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.UseSnakeCaseNamingConvention(); // ตัวแปลงชื่อใน database จาก snake_case เป็น PascalCase อัตโนมัติ
                options.UseLazyLoadingProxies(false); // Turn off EF LazyLoad
            });


            // Register Hangfire
            services.AddHangfire(config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = true // default is "true"
                    }));
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 5; // fix max worker (ทุกๆ 1 Worker ที่กำลัง Processing จะถือครอง Connection ของ DB ไว้ 1 ตัวเสมอ)
            });

            //services.AddScoped<IAppUnitOfWork, EfUnitOfWork>();

            services.Scan(scan => scan
                    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.FullName != null && a.FullName.Contains("HigzTrade.Infrastructure")))
                    .AddClasses(classes => classes
                        .Where(type => type.Namespace != null && type.Namespace.StartsWith("HigzTrade.Infrastructure"))
                        .Where(type => type.Name.EndsWith("Repository") ||
                                               type.Name.EndsWith("Service") ||
                                               type.Name.EndsWith("Query") ||
                                               type.Name.EndsWith("Job") ||
                                               type.Name.EndsWith("UnitOfWork"))
                    )
                    .AsImplementedInterfaces()
                    //.AsSelf()
                    .WithScopedLifetime()
                    );


            return services;
        }
    }
}
