namespace Domain.Exceptions;

public class IpRateLimitException : BaseException
{
    public IpRateLimitException() : base($"You have reached the request limit. Try again later.", 429)
    {
    }

    public IpRateLimitException(double totalMinutes) : base($"You have reached the request limit. Try again in {totalMinutes} minutes.", 429)
    {
    }
}