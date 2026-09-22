namespace XtramileWeather.Application.Weather;

public sealed class WeatherDto
{
    public int CityId { get; init; }
    public string CityName { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;

    public double TemperatureCelsius { get; init; }
    public double TemperatureFahrenheit { get; init; }

    public double FeelsLikeCelsius { get; init; }
    public double FeelsLikeFahrenheit { get; init; }

    public double MinimumTemperatureCelsius { get; init; }
    public double MinimumTemperatureFahrenheit { get; init; }

    public double MaximumTemperatureCelsius { get; init; }
    public double MaximumTemperatureFahrenheit { get; init; }

    public double DewPointCelsius { get; init; }
    public double DewPointFahrenheit { get; init; }

    public int Humidity { get; init; }
    public int PressureHpa { get; init; }
    public int VisibilityMeters { get; init; }

    public double WindSpeedMetersPerSecond { get; init; }

    public string Sky { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public DateTimeOffset ObservedAtUtc { get; init; }
}