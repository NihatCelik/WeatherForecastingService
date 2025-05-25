using Infrastructure.Models;

namespace Infrastructure.Abstract;

public interface IIpLocationService
{
    Task<IpLocationResponse> GetLocationAsync(string clientIp);
}