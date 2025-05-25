using Domain.Exceptions;
using System.Threading.RateLimiting;

namespace WebAPI.ServiceCollectionExtensions;

public static class RateLimitExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpcontext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromSeconds(30)
                }
            ));

            options.OnRejected = (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    throw new IpRateLimitException(retryAfter.TotalMinutes);
                }
                else
                {
                    throw new IpRateLimitException();
                }
            };
        });
        return services;
    }
}