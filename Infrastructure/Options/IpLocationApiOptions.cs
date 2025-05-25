namespace Infrastructure.Options;

public class IpLocationApiOptions
{
    public const string SectionName = "IpLocationApi";
    public const string HttpClientName = "IpLocationClient";

    public string BaseAddress { get; set; }
}