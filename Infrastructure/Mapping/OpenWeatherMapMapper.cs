using Domain.Models;
using Infrastructure.Models;

namespace Infrastructure.Mapping;

public static class OpenWeatherMapMapper
{
    public static WeatherResponse ToWeatherResponse(OpenWeatherMapApiResponse response)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(response.Dt);
        DateTime dateTime = dateTimeOffset.LocalDateTime;
        return new WeatherResponse
        {
            Region = response.Name,
            Date = dateTime,
            TemperatureCelsius = response.Main.Temp,
            TemperatureFeelsCelsius = response.Main.FeelsLike,
            Humidity = response.Main.Humidity,
            WindSpeedKph = response.Wind.Speed * 3.6, // Convert m/s to km/h
            Weather = response.Weather.FirstOrDefault()?.Description ?? "No description available",
            IconUrl = $"https://openweathermap.org/img/w/{response.Weather.FirstOrDefault()?.Icon}.png"
        };
    }
}