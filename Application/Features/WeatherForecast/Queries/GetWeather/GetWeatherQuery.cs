using Infrastructure.Enums;
using MediatR;

namespace Application.Features.WeatherForecast.Queries.GetWeather;

public class GetWeatherQuery : IRequest<GetWeatherResponse>
{
    public string City { get; set; }
    public DateTime? Date { get; set; }
    public UnitTypes UnitType { get; set; }
}