using Application.Features.WeatherForecast.Queries.GetWeather;
using FluentValidation;

namespace Application.Features.WeatherForecast.ValidationRules;

public class GetWeatherRequestValidator : AbstractValidator<GetWeatherQuery>
{
    public GetWeatherRequestValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MinimumLength(3).WithMessage("City name is too short")
            .MaximumLength(100).WithMessage("City name is too long");

        RuleFor(x => x.Date)
            .GreaterThan(new DateTime(2000)).WithMessage("The date must be later than January 1, 2000.")
            .LessThan(new DateTime(2030)).WithMessage("The date must be earlier than January 1, 2030.");

        RuleFor(x => x.UnitType)
            .IsInEnum().WithMessage("Unit Type must be a valid enum value");
    }
}