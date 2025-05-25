namespace Domain.Exceptions;

public abstract class BaseException(string message, int statusCode = 500)
    : Exception(message)
{
    public virtual int StatusCode { get; } = statusCode;
}