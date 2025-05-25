using Application.Features.WeatherForecast.Queries.GetCurrentWeather;
using Domain.Models;
using Infrastructure.Abstract;
using Infrastructure.Models;
using Moq;
using Xunit;

namespace Tests;

public class GetCurrentWeatherQueryHandlerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly Mock<IIpLocationService> _ipLocationServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly GetCurrentWeatherQueryHandler _handler;

    public GetCurrentWeatherQueryHandlerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _ipLocationServiceMock = new Mock<IIpLocationService>();
        _cacheServiceMock = new Mock<ICacheService>();

        _handler = new GetCurrentWeatherQueryHandler(
            _weatherServiceMock.Object,
            _ipLocationServiceMock.Object,
            _cacheServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedResult_IfExists()
    {
        // Arrange
        var query = new GetCurrentWeatherQuery { Ip = "192.168.1.1", UnitType = Infrastructure.Enums.UnitTypes.Metric };
        var mockLocation = new IpLocationResponse { Lat = 52.37, Lon = 4.89 };
        var mockWeather = GetMockWeatherResponse("Amsterdam");

        _ipLocationServiceMock
            .Setup(x => x.GetLocationAsync(query.Ip))
            .ReturnsAsync(mockLocation);

        _cacheServiceMock
            .Setup(x => x.GetOrAddAsync(
                It.Is<string>(s => s.Contains(query.Ip)),
                It.IsAny<Func<Task<WeatherResponse>>>(),
                It.IsAny<TimeSpan>()))
            .ReturnsAsync(mockWeather);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("Amsterdam", result.City);
        Assert.Equal(mockWeather.TemperatureCelsius, result.TemperatureCelsius);
    }

    [Fact]
    public async Task Handle_ShouldCallWeatherService_IfCacheMissOccurs()
    {
        // Arrange
        var query = new GetCurrentWeatherQuery { Ip = "192.168.1.1", UnitType = Infrastructure.Enums.UnitTypes.Fahreinheit };
        var mockLocation = new IpLocationResponse { Lat = 51.92, Lon = 4.48 };
        var mockWeather = GetMockWeatherResponse("Rotterdam");

        _ipLocationServiceMock
            .Setup(x => x.GetLocationAsync(query.Ip))
            .ReturnsAsync(mockLocation);

        _weatherServiceMock
            .Setup(x => x.GetCurrentWeatherAsync(mockLocation.Lat, mockLocation.Lon, query.UnitType))
            .ReturnsAsync(mockWeather);

        _cacheServiceMock
            .Setup(x => x.GetOrAddAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<WeatherResponse>>>(),
                It.IsAny<TimeSpan>()))
            .Returns<string, Func<Task<WeatherResponse>>, TimeSpan>((_, factory, _) => factory());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _weatherServiceMock.Verify(x => x.GetCurrentWeatherAsync(mockLocation.Lat, mockLocation.Lon, query.UnitType), Times.Once);
        Assert.Equal("Rotterdam", result.City);
    }

    [Fact]
    public async Task Handle_ShouldHandleNullLocation()
    {
        // Arrange
        var query = new GetCurrentWeatherQuery { Ip = "invalid-ip", UnitType = Infrastructure.Enums.UnitTypes.Metric };

        _ipLocationServiceMock
            .Setup(x => x.GetLocationAsync(query.Ip))
            .ReturnsAsync((IpLocationResponse)null!); // simulates not found

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var query = new GetCurrentWeatherQuery { Ip = "192.168.1.3", UnitType = Infrastructure.Enums.UnitTypes.Metric };
        var location = new IpLocationResponse { Lat = 52.08, Lon = 5.12 };
        var mockWeather = GetMockWeatherResponse("Utrecht");

        _ipLocationServiceMock
            .Setup(x => x.GetLocationAsync(query.Ip))
            .ReturnsAsync(location);

        _cacheServiceMock
            .Setup(x => x.GetOrAddAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<WeatherResponse>>>(),
                It.IsAny<TimeSpan>()))
            .ReturnsAsync(mockWeather);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(mockWeather.Region, result.City);
        Assert.Equal(mockWeather.TemperatureCelsius, result.TemperatureCelsius);
        Assert.Equal(mockWeather.Weather, result.Weather);
        Assert.Equal(mockWeather.Humidity, result.Humidity);
        Assert.Equal(mockWeather.WindSpeedKph, result.WindSpeedKph);
        Assert.Equal(mockWeather.Date, result.Date);
        Assert.Equal(mockWeather.IconUrl, result.IconUrl);
    }

    private static WeatherResponse GetMockWeatherResponse(string city) => new()
    {
        Region = city,
        TemperatureCelsius = 20,
        Weather = "Sunny",
        Humidity = 60,
        WindSpeedKph = 12.5,
        Date = DateTime.Today,
        IconUrl = $"http://icon.com/{city.ToLower()}.png"
    };
}