using Domain.Models;
using Infrastructure.Abstract;
using Infrastructure.Enums;
using Infrastructure.Mapping;
using Infrastructure.Models;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.WeatherServices;

public class OpenWeatherMapService(IHttpClientFactory httpClientFactory, IOptions<OpenWeatherMapApiOptions> options) : IWeatherService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(OpenWeatherMapApiOptions.HttpClientName);
    private readonly OpenWeatherMapApiOptions _options = options.Value;

    public async Task<WeatherResponse> GetCurrentWeatherAsync(double latitude, double longitude, UnitTypes unitType)
    {
        var url = $"{_options.BaseAddress}weather?appid={_options.ApiKey}&lat={latitude.ToString().Replace(",", ".")}&lon={longitude.ToString().Replace(",", ".")}";
        if (unitType != UnitTypes.Default)
        {
            var unit = GetUnit(unitType);
            url += $"&units={unit}";
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var jsonResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<OpenWeatherMapApiResponse>(jsonResult);
        return OpenWeatherMapMapper.ToWeatherResponse(result);
    }

    public Task<WeatherResponse> GetWeatherAsync(string city, DateTime? date, UnitTypes unitType)
    {
        throw new NotImplementedException();
    }

    private static string GetUnit(UnitTypes unitType)
    {
        return unitType switch
        {
            UnitTypes.Metric => "metric",
            UnitTypes.Fahreinheit => "imperial",
            _ => "metric",
        };
    }
}