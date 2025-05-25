using Domain.Exceptions;
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

        return await GetApiResponse(url);
    }

    public async Task<WeatherResponse> GetWeatherAsync(string city, DateTime? date, UnitTypes unitType)
    {
        var url = $"{_options.BaseAddress}?appid={_options.ApiKey}&q={city}";
        if (date.HasValue)
        {
            var start = new DateTimeOffset(date.Value.Date).ToUnixTimeMilliseconds();
            var end = new DateTimeOffset(date.Value.Date.AddDays(1)).ToUnixTimeMilliseconds();
            url = $"{_options.HistoryBaseAddress}city?q={city}&type=hour&start={start}&end={end}&appid={_options.ApiKey}";
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
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessAppException("OpenWeatherMap");
        }
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new OpenWeatherMapServiceException($"OpenWeatherMapService API error: Content: {errorContent}", response.StatusCode.GetHashCode());
        }

        var jsonResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<OpenWeatherMapApiResponse>(jsonResult);
        return OpenWeatherMapMapper.ToWeatherResponse(result);
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