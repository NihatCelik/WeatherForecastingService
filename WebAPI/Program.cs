using Application.Features.WeatherForecast.Queries.GetCurrentWeather;
using FluentValidation;
using MediatR.Extensions.FluentValidation.AspNetCore;
using MediatR;
using WebAPI.ServiceCollectionExtensions;
using Application.Features.WeatherForecast.ValidationRules;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<GetWeatherRequestValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetCurrentWeatherQueryHandler).Assembly));

builder.Services.AddMemoryCache();
builder.Services.AddCustomRateLimiting();
builder.AddExternalHttpClientServices();
builder.AddOptions();
builder.Services.AddAppServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();