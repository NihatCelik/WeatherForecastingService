using Application.Features.WeatherForecast.Queries.GetWeather;
using Domain.Models;
using Infrastructure.Abstract;
using Moq;
using Xunit;

namespace Tests;

public class GetWeatherQueryHandlerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly GetWeatherQueryHandler _handler;

    public GetWeatherQueryHandlerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _cacheServiceMock = new Mock<ICacheService>();
        _handler = new GetWeatherQueryHandler(_weatherServiceMock.Object, _cacheServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnWeatherFromCache_WhenDataIsCached()
    {
        // Arrange
        var query = new GetWeatherQuery { City = "Amsterdam", Date = DateTime.Today, UnitType = Infrastructure.Enums.UnitTypes.Metric };
        var cachedResult = GetMockWeatherResult("Amsterdam");

        _cacheServiceMock
            .Setup(c => c.GetOrAddAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<WeatherResponse>>>(),
                It.IsAny<TimeSpan>()))
            .ReturnsAsync(cachedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(cachedResult.Region, result.City);
        Assert.Equal(cachedResult.TemperatureCelsius, result.TemperatureCelsius);
        Assert.Equal(cachedResult.Weather, result.Weather);
    }

    [Fact]
    public async Task Handle_ShouldCallWeatherService_WhenCacheMissOccurs()
    {
        // Arrange
        var query = new GetWeatherQuery { City = "Rotterdam", Date = DateTime.Today, UnitType = Infrastructure.Enums.UnitTypes.Fahreinheit };
        var serviceResult = GetMockWeatherResult("Rotterdam");

        _weatherServiceMock
            .Setup(ws => ws.GetWeatherAsync(query.City, query.Date, query.UnitType))
            .ReturnsAsync(serviceResult);

        _cacheServiceMock
            .Setup(c => c.GetOrAddAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<WeatherResponse>>>(),
                It.IsAny<TimeSpan>()))
            .Returns<string, Func<Task<WeatherResponse>>, TimeSpan>((_, factory, _) => factory());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _weatherServiceMock.Verify(ws => ws.GetWeatherAsync(query.City, query.Date, query.UnitType), Times.Once);
        Assert.Equal(serviceResult.Region, result.City);
    }

    [Fact]
    public async Task Handle_ShouldHandleNullDate()
    {
        // Arrange
        var query = new GetWeatherQuery { City = "Utrecht", UnitType = Infrastructure.Enums.UnitTypes.Metric };
        var expectedResult = GetMockWeatherResult("Utrecht");

        _cacheServiceMock
            .Setup(c => c.GetOrAddAsync(
                It.Is<string>(key => key.Contains("GetWeather_Utrecht__Metric")),
                It.IsAny<Func<Task<WeatherResponse>>>(),
                It.IsAny<TimeSpan>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult.Region, result.City);
    }

    [Fact]
    public async Task Handle_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var query = new GetWeatherQuery { City = "Eindhoven", Date = DateTime.Today, UnitType = Infrastructure.Enums.UnitTypes.Metric };
        var expected = GetMockWeatherResult("Eindhoven");

        _cacheServiceMock
            .Setup(c => c.GetOrAddAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<WeatherResponse>>>(),
                It.IsAny<TimeSpan>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expected.Region, result.City);
        Assert.Equal(expected.TemperatureCelsius, result.TemperatureCelsius);
        Assert.Equal(expected.Weather, result.Weather);
        Assert.Equal(expected.Humidity, result.Humidity);
        Assert.Equal(expected.WindSpeedKph, result.WindSpeedKph);
        Assert.Equal(expected.Date, result.Date);
        Assert.Equal(expected.IconUrl, result.IconUrl);
    }

    private static WeatherResponse GetMockWeatherResult(string city) => new()
    {
        Region = city,
        TemperatureCelsius = 18.2,
        Weather = "Cloudy",
        Humidity = 75,
        WindSpeedKph = 15.5,
        Date = DateTime.Today,
        IconUrl = $"http://icon.com/{city.ToLower()}.png"
    };
}