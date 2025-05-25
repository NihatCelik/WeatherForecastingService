namespace Domain.Models;

public class WeatherResponse
{
    public string Region { get; set; }
    public DateTime Date { get; set; }
    public double TemperatureCelsius { get; set; }
    public double TemperatureFeelsCelsius { get; set; }
    public string Weather { get; set; }
    public int Humidity { get; set; }
    public double WindSpeedKph { get; set; }
    public string IconUrl { get; set; }
}