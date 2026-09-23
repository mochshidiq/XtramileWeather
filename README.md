# Xtramile Weather

A weather application built with ASP.NET Core .NET 8, Entity Framework Core, SQLite, MediatR, and a lightweight HTML, CSS, and JavaScript frontend.

The application allows users to select a country and city, view current weather information from OpenWeatherMap, switch between Celsius and Fahrenheit, and manage favorite cities.

## Features

- Display available countries
- Filter cities based on the selected country
- Retrieve current weather from OpenWeatherMap
- Display temperature in Celsius and Fahrenheit
- Display feels-like, minimum, and maximum temperatures
- Display humidity, pressure, visibility, wind speed, and sky condition
- Calculate dew point
- Add and remove favorite cities
- Prevent duplicate favorites
- Friendly API error handling
- Swagger API documentation
- Automated tests using xUnit

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQLite
- MediatR
- OpenWeatherMap API
- xUnit
- HTML
- CSS
- JavaScript

## Project Structure

```text
XtramileWeather
├── XtramileWeather.Domain
│   └── Domain entities
├── XtramileWeather.Application
│   └── Business logic, CQRS, MediatR, DTOs, and interfaces
├── XtramileWeather.Infrastructure
│   └── Database, migrations, and OpenWeatherMap integration
├── XtramileWeather.Api
│   └── API controllers, middleware, configuration, and frontend
└── XtramileWeather.Application.Tests
    └── Automated application tests
```

## Architecture

The solution uses a simplified Clean Architecture structure:

- **Domain** contains the main entities.
- **Application** contains commands, queries, handlers, DTOs, and interfaces.
- **Infrastructure** implements database access and external weather integration.
- **API** provides HTTP endpoints and serves the frontend.
- **Application.Tests** contains automated tests for application handlers.

## Prerequisites

Install the following tools before running the application:

- .NET 8 SDK
- Visual Studio 2022 or another compatible editor
- An OpenWeatherMap account and API key
- DB Browser for SQLite (optional)

## OpenWeatherMap API Key

Create an API key at:

https://openweathermap.org/api

The API key must not be stored directly in source code or committed to Git.

Set the API key using .NET User Secrets:

```bash
dotnet user-secrets init --project XtramileWeather.Api
dotnet user-secrets set "OpenWeather:ApiKey" "YOUR_API_KEY" --project XtramileWeather.Api
```

A newly created OpenWeatherMap API key may require some time before it becomes active.

## Database Setup

Restore the project dependencies:

```bash
dotnet restore
```

Apply the Entity Framework Core migration:

```bash
dotnet ef database update --project XtramileWeather.Infrastructure --startup-project XtramileWeather.Api
```

The migration creates the SQLite database and inserts the initial country and city data.

## Running the Application

Run the API from the solution directory:

```bash
dotnet run --project XtramileWeather.Api
```

The terminal will display the application URL.

Open the frontend using the HTTPS URL, for example:

```text
https://localhost:7163/
```

Swagger can be opened at:

```text
https://localhost:7163/swagger
```

The port number may differ depending on the local configuration.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/countries` | Get all countries |
| GET | `/api/cities?countryId={id}` | Get cities filtered by country |
| GET | `/api/weather/{cityId}` | Get current weather for a city |
| GET | `/api/favorite-cities` | Get all favorite cities |
| POST | `/api/favorite-cities/{cityId}` | Add a city to favorites |
| DELETE | `/api/favorite-cities/{favoriteId}` | Remove a favorite city |

## Running Automated Tests

Run all automated tests using:

```bash
dotnet test
```

The tests cover:

- Retrieving and sorting countries
- Filtering cities by country
- Adding a favorite city
- Rejecting duplicate favorites
- Removing a favorite city
- Calling the weather service with the correct city information

Current test result:

```text
6 passed
0 failed
0 skipped
```

## Temperature Conversion

Weather data is requested using metric units.

Fahrenheit values are calculated using:

```text
°F = (°C × 9 / 5) + 32
```

The dew point is calculated from temperature and relative humidity using the Magnus formula.

## Assumptions

- The application is intended as a single-user demonstration.
- Authentication and user accounts are outside the project scope.
- Favorite cities are shared within the local SQLite database.
- Countries and cities use seeded data.
- Each city can only be added to favorites once.
- Weather data is retrieved from OpenWeatherMap and is not stored permanently.
- SQLite is used to simplify local setup.
- The frontend uses plain HTML, CSS, and JavaScript.

## Security

- Do not commit the OpenWeatherMap API key.
- Store secrets using .NET User Secrets or environment variables.
- Do not expose detailed internal errors in production.