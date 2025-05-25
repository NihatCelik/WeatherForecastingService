using Application.Features.WeatherForecast.Queries.GetCurrentWeather;
using Application.Features.WeatherForecast.Queries.GetWeather;
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

    [HttpGet("GetWeather")]
    public async Task<IActionResult> GetWeather([FromQuery] GetWeatherQuery request)
    {
        var result = await Mediator.Send(request);
        return Ok(result);
    }
}