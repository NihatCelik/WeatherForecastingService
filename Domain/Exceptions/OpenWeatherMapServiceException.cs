namespace Domain.Exceptions;

public class OpenWeatherMapServiceException(string message, int httpCode)
 : BaseException(message, httpCode)
{
}