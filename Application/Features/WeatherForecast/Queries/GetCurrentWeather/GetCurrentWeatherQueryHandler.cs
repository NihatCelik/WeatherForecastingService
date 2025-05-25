using Infrastructure.Abstract;
using MediatR;

namespace Application.Features.WeatherForecast.Queries.GetCurrentWeather;

public class GetCurrentWeatherQueryHandler(IWeatherService weatherService, IIpLocationService ipLocationService, ICacheService cacheService) : IRequestHandler<GetCurrentWeatherQuery, GetCurrentWeatherResponse>
{
    public async Task<GetCurrentWeatherResponse> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
    {
        string cacheName = $"GetCurrentWeather_{request.Ip}_{request.UnitType}";

        var location = await ipLocationService.GetLocationAsync(request.Ip);
        var response = await cacheService.GetOrAddAsync(
            cacheName,
            async () => await weatherService.GetCurrentWeatherAsync(location.Lat, location.Lon, request.UnitType),
            TimeSpan.FromMinutes(30));

        return new GetCurrentWeatherResponse
        {
            City = response.Region,
            TemperatureCelsius = response.TemperatureCelsius,
            Weather = response.Weather,
            Humidity = response.Humidity,
            WindSpeedKph = response.WindSpeedKph,
            Date = response.Date,
            IconUrl = response.IconUrl
        };
    }
}