using Domain.Exceptions;
using FluentValidation;
using Newtonsoft.Json;

namespace WebAPI.Middlewares;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errorDetails = new
            {
                message = "Validation failed",
                errors = ex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
            };

            var json = JsonConvert.SerializeObject(errorDetails);

            if (!context.Response.HasStarted)
                await context.Response.WriteAsJsonAsync(errorDetails);
        }
        catch (CityNotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            logger.LogWarning(ex, "City not found!");
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                status = context.Response.StatusCode
            });
        }
        catch (UnauthorizedAccessAppException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            logger.LogWarning(ex, "App key is wrong!");
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                status = context.Response.StatusCode
            });
        }
        catch (WeatherStackServiceException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            logger.LogWarning(ex, "WeatherStackApi Exception!");
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                status = context.Response.StatusCode
            });
        }
        catch (OpenWeatherMapServiceException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            logger.LogWarning(ex, "OpenWeatherMap Exception!");
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                status = context.Response.StatusCode
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            logger.LogError(ex, "Unhandled exception");
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Internal server error",
                status = 500
            });
        }
    }
}