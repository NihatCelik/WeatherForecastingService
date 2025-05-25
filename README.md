# 🌤️ Weather API - ASP.NET Core 8

## 📌 Project Description

**Weather API** is a secure, high-performance, and scalable RESTful service built with ASP.NET Core. It provides real-time weather data from external providers (e.g., OpenWeatherMap). The project is architected using senior-level best practices, SOLID principles, and modern .NET patterns.

## 🛠️ Technologies & Features

- **ASP.NET Core 8**
- **C# 12**
- **RESTful API Design**
- **Swagger** (API documentation)
- **Serilog** (Logging to Elasticsearch + visualization via Kibana)
- **MediatR** (CQRS pattern)
- **FluentValidation** (Robust request validation)
- **Custom Exception Middleware** (Centralized error handling)
- **IpRateLimit** (Rate limiting per IP)
- **Caching** (In-memory)
- **xUnit / Moq** (Unit & integration testing)
- **Docker / Docker Compose**

## 🧱 Project Architecture

- **API Layer**: Controllers, middleware, filters
- **Application Layer**: CQRS commands/queries, validators
- **Domain Layer**: Core business logic and entities
- **Infrastructure Layer**: External services, logging, persistence
- **Test Layer**: Unit and integration tests

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Elasticsearch & Kibana (optional)](https://www.elastic.co/)

### Running Locally

```bash
git clone https://github.com/your-username/WeatherForecastingService.git
cd WeatherForecastingService
dotnet build
dotnet run --project src/WeatherForecastingService.WebAPI
```
### Running with Docker
```
docker-compose up --build
```

## ⚙️ Configuration
Configure environment values via appsettings.Development.json or a .env file:
```
{
  "OpenWeatherMapApi": {
    "BaseAddress": "https://api.openweathermap.org/data/2.5/weather",
    "HistoryBaseAddress": "https://history.openweathermap.org/data/2.5/history/",
    "ApiKey": "bd5e378503939ddaee76f12ad7a97608" // Replace with your actual OpenWeatherMap API key
  },
  "WeatherStackApi": {
    "BaseAddress": "https://api.weatherstack.com/current",
    "HistoryBaseAddress": "http://api.weatherstack.com/historical",
    "ApiKey": "b8c022e724e468da18f91e62f8befc38" // Replace with your actual WeatherStack API key
  },
  "IpLocationApi": {
    "BaseAddress": "http://ip-api.com/json/"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System": "Warning"
      }
    }
  },
  "ElasticConfiguration": {
    "Uri": "http://localhost:9200"
  }
}
```
## 📦 API Documentation
Once running, visit the following for API exploration:
```
http://localhost:5000/swagger
```

## 🧪 Testing
Run tests using:
```
dotnet test
```
Tests are located under the tests/ directory and include both unit and integration tests.

## 📊 Logging & Monitoring
Serilog is configured to push structured logs to Elasticsearch. Kibana can be used for visualizing logs.

Kibana URL: ```http://localhost:5601```
