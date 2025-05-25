using Infrastructure.Abstract;
using Infrastructure.Caching;
using Infrastructure.LocationServices;

namespace WebAPI.ServiceCollectionExtensions;

public static class AppServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IIpLocationService, IpLocationService>();
        services.AddScoped<ICacheService, MemoryCacheService>();
        return services;
    }
}