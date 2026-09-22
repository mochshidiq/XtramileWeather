namespace XtramileWeather.Application.Weather;

public sealed class WeatherDto
{
    public int CityId { get; init; }
    public string CityName { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;

    public double TemperatureCelsius { get; init; }
    public double FeelsLikeCelsius { get; init; }
    public double MinimumTemperatureCelsius { get; init; }
    public double MaximumTemperatureCelsius { get; init; }

    public int Humidity { get; init; }
    public double WindSpeedMetersPerSecond { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTimeOffset ObservedAtUtc { get; init; }
}