using Application.Options;
using Infrastructure.Options;

namespace WebAPI.ServiceCollectionExtensions;

public static class OptionsServiceCollectionExtensions
{
    public static void AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<IpLocationApiOptions>(builder.Configuration.GetSection(IpLocationApiOptions.SectionName));
        builder.Services.Configure<OpenWeatherMapApiOptions>(builder.Configuration.GetSection(OpenWeatherMapApiOptions.SectionName));
        builder.Services.Configure<WeatherStackApiOptions>(builder.Configuration.GetSection(WeatherStackApiOptions.SectionName));
    }
}