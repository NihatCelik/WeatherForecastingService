namespace Domain.Exceptions;

public class UnauthorizedAccessAppException(string service)
    : BaseException($"{service} Key is not correct!", 401)
{
}