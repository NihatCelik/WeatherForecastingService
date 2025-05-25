using MediatR;

namespace Application.Features.WeatherForecast.Queries.GetCurrentWeather;

public class GetCurrentWeatherQueryHandler : IRequestHandler<GetCurrentWeatherQuery, GetCurrentWeatherResponse>
{
    public async Task<GetCurrentWeatherResponse> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
    {
        return new GetCurrentWeatherResponse();
    }
}