using Application.Options;
using Infrastructure.Abstract;
using Infrastructure.Enums;
using Infrastructure.Options;
using Infrastructure.WeatherServices;

namespace WebAPI.ServiceCollectionExtensions;

public static class HttpClientServiceCollectionExtensions
{
    public static void AddExternalHttpClientServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient(IpLocationApiOptions.HttpClientName, client =>
        {
            var config = builder.Configuration.GetSection(IpLocationApiOptions.SectionName).Get<IpLocationApiOptions>();
            client.BaseAddress = new Uri(config.BaseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        var activeMapApi = builder.Configuration.GetValue<WeatherApiTypes>("ActiveWeatherApi");
        if (activeMapApi == WeatherApiTypes.OpenWeatherMap)
        {
            builder.Services.AddScoped<IWeatherService, OpenWeatherMapService>();
            builder.Services.AddHttpClient(OpenWeatherMapApiOptions.HttpClientName, client =>
            {
                var config = builder.Configuration.GetSection(OpenWeatherMapApiOptions.SectionName).Get<OpenWeatherMapApiOptions>();
                client.BaseAddress = new Uri(config.BaseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }
        else if (activeMapApi == WeatherApiTypes.WeatherStack)
        {
            builder.Services.AddScoped<IWeatherService, WeatherStackService>();
            builder.Services.AddHttpClient(WeatherStackApiOptions.HttpClientName, client =>
            {
                var config = builder.Configuration.GetSection(WeatherStackApiOptions.SectionName).Get<WeatherStackApiOptions>();
                client.BaseAddress = new Uri(config.BaseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }
    }
}