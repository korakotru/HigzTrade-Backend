using Microsoft.Extensions.DependencyInjection;

namespace HigzTrade.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.Scan(scan => scan
                    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.FullName != null && a.FullName.Contains("HigzTrade.Application")))
                    .AddClasses(classes => classes
                        .Where(type => type.Namespace != null && type.Namespace.StartsWith("HigzTrade.Application.Services"))
                        .Where(type => type.Name.EndsWith("UseCase") || type.Name.EndsWith("Query"))
                    )
                    //.AsImplementedInterfaces()
                    .AsSelf()
                    .WithScopedLifetime()
                    );

            return services;
        }
    }
}
