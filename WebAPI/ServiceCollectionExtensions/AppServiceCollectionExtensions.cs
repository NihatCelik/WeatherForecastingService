using Infrastructure.Abstract;
using Infrastructure.Caching;
using Infrastructure.LocationServices;
using WebAPI.Middlewares;

namespace WebAPI.ServiceCollectionExtensions;

public static class AppServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<ExceptionMiddleware>();

        services.AddScoped<IIpLocationService, IpLocationService>();
        services.AddScoped<ICacheService, MemoryCacheService>();
        return services;
    }
}