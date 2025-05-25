namespace Application.Features.WeatherForecast.Queries.GetCurrentWeather;

public class GetCurrentWeatherResponse
{
    public string City { get; set; }
    public DateTime Date { get; set; }
    public double TemperatureCelsius { get; set; }
    public string Weather { get; set; }
    public int Humidity { get; set; }
    public double WindSpeedKph { get; set; }
    public string IconUrl { get; set; }
}