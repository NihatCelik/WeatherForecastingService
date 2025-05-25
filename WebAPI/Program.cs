using Application.Features.WeatherForecast.Queries.GetCurrentWeather;
using FluentValidation;
using MediatR.Extensions.FluentValidation.AspNetCore;
using MediatR;
using WebAPI.ServiceCollectionExtensions;
using Application.Features.WeatherForecast.ValidationRules;
using WebAPI.Middlewares;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile(
           $"appsettings.{environment}.json", optional: true
       ).Build();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Debug()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"weather-logs-{DateTime.UtcNow:yyyy.MM}"
    })
    .Enrich.WithProperty("Environment", environment)
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string>();
builder.Services.AddCors(options =>
{
    var originValues = origins.Split(";");
    options.AddPolicy("AllowOrigin",
        builder => builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(originValues));
});

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

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();