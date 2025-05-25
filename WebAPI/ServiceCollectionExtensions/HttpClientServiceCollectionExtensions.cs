using Infrastructure.Options;

namespace WebAPI.ServiceCollectionExtensions;

public static class HttpClientServiceCollectionExtensions
{
    public static void AddExternalHttpClientServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient(IpLocationApiOptions.HttpClientName, client =>
        {
            var config = builder.Configuration.GetSection(IpLocationApiOptions.SectionName).Get<IpLocationApiOptions>();
            client.BaseAddress = new Uri(config.BaseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
    }
}