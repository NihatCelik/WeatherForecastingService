namespace Application.Options;

public class WeatherStackApiOptions
{
    public const string SectionName = "WeatherStackApi";
    public const string HttpClientName = "WeatherStackApiClient";

    public string BaseAddress { get; set; }
    public string HistoryBaseAddress { get; set; }
    public string ApiKey { get; set; }
}