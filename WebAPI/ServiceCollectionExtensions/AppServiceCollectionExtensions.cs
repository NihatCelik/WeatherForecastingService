using Infrastructure.Abstract;
using Infrastructure.LocationServices;

namespace WebAPI.ServiceCollectionExtensions;

public static class AppServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IIpLocationService, IpLocationService>();
        return services;
    }
}