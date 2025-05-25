using Domain.Models;
using Infrastructure.Models;

namespace Infrastructure.Mapping;

public static class WeatherStackMapper
{
    public static WeatherResponse ToWeatherResponse(WeatherStackApiResponse response)
    {
        DateTime localTime = DateTimeOffset.FromUnixTimeSeconds(response.Location.LocaltimeEpoch).UtcDateTime;
        localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, localTime.Hour, localTime.Minute, localTime.Second, DateTimeKind.Local);

        return new WeatherResponse
        {
            Region = response.Location.Region,
            Date = localTime,
            TemperatureCelsius = response.Current.Temperature,
            TemperatureFeelsCelsius = response.Current.Feelslike,
            Humidity = response.Current.Humidity,
            WindSpeedKph = response.Current.WindSpeed,
            Weather = response.Current.WeatherDescriptions.FirstOrDefault() ?? "No description available",
            IconUrl = response.Current.WeatherIcons.FirstOrDefault() ?? string.Empty
        };
    }
}