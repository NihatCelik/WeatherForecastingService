using Infrastructure.Enums;
using MediatR;

namespace Application.Features.WeatherForecast.Queries.GetCurrentWeather;

public class GetCurrentWeatherQuery : IRequest<GetCurrentWeatherResponse>
{
    public string Ip { get; set; }
    public UnitTypes UnitType { get; set; }
}