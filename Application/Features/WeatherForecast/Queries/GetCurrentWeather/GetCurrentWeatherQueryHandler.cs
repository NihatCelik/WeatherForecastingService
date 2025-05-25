using Infrastructure.Abstract;
using MediatR;

namespace Application.Features.WeatherForecast.Queries.GetCurrentWeather;

public class GetCurrentWeatherQueryHandler(IWeatherService weatherService, IIpLocationService ipLocationService) : IRequestHandler<GetCurrentWeatherQuery, GetCurrentWeatherResponse>
{
    public async Task<GetCurrentWeatherResponse> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
    {
        var location = await ipLocationService.GetLocationAsync(request.Ip);
        var response = await weatherService.GetCurrentWeatherAsync(location.Lat, location.Lon, request.UnitType);

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