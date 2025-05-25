using Application.Options;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure.Abstract;
using Infrastructure.Enums;
using Infrastructure.Mapping;
using Infrastructure.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.WeatherServices;

public class WeatherStackService(IHttpClientFactory httpClientFactory, IOptions<WeatherStackApiOptions> options) : IWeatherService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WeatherStackApiOptions.HttpClientName);
    private readonly WeatherStackApiOptions _options = options.Value;

    public async Task<WeatherResponse> GetCurrentWeatherAsync(double latitude, double longitude, UnitTypes unitType)
    {
        var url = $"{_options.BaseAddress}?access_key={_options.ApiKey}&query={latitude.ToString().Replace(",", ".")},{longitude.ToString().Replace(",", ".")}";
        if (unitType != UnitTypes.Default)
        {
            var unit = GetUnit(unitType);
            url += $"&units={unit}";
        }

        return await GetApiResponse(url);
    }

    public async Task<WeatherResponse> GetWeatherAsync(string city, DateTime? date, UnitTypes unitType)
    {
        var url = $"{_options.BaseAddress}?access_key={_options.ApiKey}&query={city}";
        if (date.HasValue)
        {
            url = $"{_options.HistoryBaseAddress}?access_key={_options.ApiKey}&query={city}&type=hourly&historical_date={date.Value.Date:yyyy-MM-dd}";
        }
        if (unitType != UnitTypes.Default)
        {
            var unit = GetUnit(unitType);
            url += $"&units={unit}";
        }
        return await GetApiResponse(url, city);
    }

    private async Task<WeatherResponse> GetApiResponse(string url, string city = "")
    {
        var response = await _httpClient.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new CityNotFoundException(city);
        }
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new WeatherStackServiceException($"WeatherStackService API error: Content: {errorContent}", response.StatusCode.GetHashCode());
        }

        var jsonResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<WeatherStackApiResponse>(jsonResult);
        return WeatherStackMapper.ToWeatherResponse(result);
    }

    private static string GetUnit(UnitTypes unitType)
    {
        return unitType switch
        {
            UnitTypes.Metric => "m",
            UnitTypes.Fahreinheit => "f",
            _ => "m",
        };
    }
}