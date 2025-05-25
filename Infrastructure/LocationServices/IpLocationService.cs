using Infrastructure.Abstract;
using Infrastructure.Models;
using Infrastructure.Options;
using Newtonsoft.Json;

namespace Infrastructure.LocationServices;

public class IpLocationService(IHttpClientFactory httpClientFactory) : IIpLocationService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(IpLocationApiOptions.HttpClientName);

    public async Task<IpLocationResponse> GetLocationAsync(string clientIp)
    {
        var locationUrl = $"{_httpClient.BaseAddress}{clientIp}";

        var response = await _httpClient.GetAsync(locationUrl);

        response.EnsureSuccessStatusCode();

        var jsonResult = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IpLocationResponse>(jsonResult);
    }
}