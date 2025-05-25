namespace Domain.Exceptions;

public class CityNotFoundException(string city)
    : BaseException($"{city} not found!", 400)
{
}