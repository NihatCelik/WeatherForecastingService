using Infrastructure.Abstract;
using MediatR;

namespace Application.Features.WeatherForecast.Queries.GetWeather;

public class GetWeatherQueryHandler(IWeatherService weatherService, ICacheService cacheService) : IRequestHandler<GetWeatherQuery, GetWeatherResponse>
{
    private readonly IWeatherService _weatherService = weatherService;

    public async Task<GetWeatherResponse> Handle(GetWeatherQuery request, CancellationToken cancellationToken)
    {
        string cacheName = $"GetWeather_{request.City}_{request.Date?.ToString("yyyyMMdd")}_{request.UnitType}";
        var response = await cacheService.GetOrAddAsync(
            cacheName,
            async () => await _weatherService.GetWeatherAsync(request.City, request.Date, request.UnitType),
            TimeSpan.FromMinutes(30));

        return new GetWeatherResponse
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