namespace Infrastructure.Options;

public class OpenWeatherMapApiOptions
{
    public const string SectionName = "OpenWeatherMapApi";
    public const string HttpClientName = "OpenWeatherMapClient";

    public string BaseAddress { get; set; }
    public string HistoryBaseAddress { get; set; }
    public string ApiKey { get; set; }
}