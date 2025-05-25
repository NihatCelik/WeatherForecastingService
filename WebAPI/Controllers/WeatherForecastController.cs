using Application.Features.WeatherForecast.Queries.GetCurrentWeather;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class WeatherForecastController : BaseApiController
{
    [HttpGet("GetCurrentWeather")]
    public async Task<IActionResult> GetCurrentWeather([FromQuery] UnitTypes unitType, CancellationToken cancellationToken)
    {
        var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var result = await Mediator.Send(new GetCurrentWeatherQuery { Ip = ip, UnitType = unitType }, cancellationToken);
        return Ok(result);
    }
}