using Infrastructure.Abstract;
using Infrastructure.Models;
using Infrastructure.Options;
using Newtonsoft.Json;

namespace Infrastructure.LocationServices;

public class IpLocationService(IHttpClientFactory httpClientFactory, ICacheService cacheService) : IIpLocationService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(IpLocationApiOptions.HttpClientName);

    public async Task<IpLocationResponse> GetLocationAsync(string clientIp)
    {
        string cacheName = $"GetLocation_{clientIp}";
        var locationUrl = $"{_httpClient.BaseAddress}{clientIp}";

        var response = await cacheService.GetOrAddAsync(
            cacheName,
            async () => await _httpClient.GetAsync(locationUrl),
            TimeSpan.FromMinutes(10));

        response.EnsureSuccessStatusCode();

        var jsonResult = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IpLocationResponse>(jsonResult);
    }
}