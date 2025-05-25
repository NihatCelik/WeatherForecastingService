using Domain.Models;
using Infrastructure.Abstract;
using Infrastructure.Enums;

namespace Infrastructure.WeatherServices;

public class WeatherStackService : IWeatherService
{
    public Task<WeatherResponse> GetCurrentWeatherAsync(double latitude, double longitude, UnitTypes unitType)
    {
        throw new NotImplementedException();
    }
}