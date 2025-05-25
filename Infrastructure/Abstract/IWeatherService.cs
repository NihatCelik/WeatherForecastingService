using Domain.Models;
using Infrastructure.Enums;

namespace Infrastructure.Abstract;

public interface IWeatherService
{
    Task<WeatherResponse> GetCurrentWeatherAsync(double latitude, double longitude, UnitTypes unitType);
}