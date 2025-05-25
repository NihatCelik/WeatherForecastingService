namespace Domain.Exceptions;

public class WeatherStackServiceException(string message, int httpCode)
 : BaseException(message, httpCode)
{
}