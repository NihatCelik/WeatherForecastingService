using Application.Features.WeatherForecast.Queries.GetCityWeather;
using Infrastructure.Abstract;
using MediatR;

namespace Application.Features.WeatherForecast.Queries.GetWeather;

public class GetWeatherQueryHandler(IWeatherService weatherService) : IRequestHandler<GetWeatherQuery, GetWeatherResponse>
{
    private readonly IWeatherService _weatherService = weatherService;

    public async Task<GetWeatherResponse> Handle(GetWeatherQuery request, CancellationToken cancellationToken)
    {
        var response = await _weatherService.GetWeatherAsync(request.City, request.Date, request.UnitType);
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