using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using XtramileWeather.Application.Common.Exceptions;
using XtramileWeather.Application.Common.Interfaces;
using XtramileWeather.Application.Common.Utilities;
using XtramileWeather.Application.Weather;

namespace XtramileWeather.Infrastructure.Weather;

public sealed class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly OpenWeatherOptions _options;

    public OpenWeatherService(
        HttpClient httpClient,
        IOptions<OpenWeatherOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<WeatherDto> GetCurrentAsync(
        int cityId,
        string cityName,
        string countryCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new ExternalWeatherException(
                "OpenWeather API key is not configured.");
        }

        var location = Uri.EscapeDataString(
            $"{cityName},{countryCode}");

        var url =
            $"weather?q={location}" +
            $"&appid={Uri.EscapeDataString(_options.ApiKey)}" +
            "&units=metric";

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.GetAsync(
                url,
                cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new ExternalWeatherException(
                "Unable to contact the weather provider.",
                innerException: exception);
        }
        catch (TaskCanceledException exception)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new ExternalWeatherException(
                "The weather provider request timed out.",
                innerException: exception);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalWeatherException(
                "The weather provider rejected the request.",
                (int)response.StatusCode);
        }

        OpenWeatherResponse data;

        try
        {
            data =
                await response.Content
                    .ReadFromJsonAsync<OpenWeatherResponse>(
                        cancellationToken: cancellationToken)
                ?? throw new ExternalWeatherException(
                    "The weather provider returned an empty response.");
        }
        catch (JsonException exception)
        {
            throw new ExternalWeatherException(
                "The weather provider returned invalid data.",
                innerException: exception);
        }

        var condition = data.Weather.FirstOrDefault();

        var dewPointCelsius =
            WeatherCalculator.CalculateDewPointCelsius(
                data.Main.Temperature,
                data.Main.Humidity);

        return new WeatherDto
        {
            CityId = cityId,
            CityName = cityName,
            CountryCode = countryCode,

            TemperatureCelsius =
                WeatherCalculator.Round(data.Main.Temperature),

            TemperatureFahrenheit =
                WeatherCalculator.CelsiusToFahrenheit(
                    data.Main.Temperature),

            FeelsLikeCelsius =
                WeatherCalculator.Round(data.Main.FeelsLike),

            FeelsLikeFahrenheit =
                WeatherCalculator.CelsiusToFahrenheit(
                    data.Main.FeelsLike),

            MinimumTemperatureCelsius =
                WeatherCalculator.Round(
                    data.Main.MinimumTemperature),

            MinimumTemperatureFahrenheit =
                WeatherCalculator.CelsiusToFahrenheit(
                    data.Main.MinimumTemperature),

            MaximumTemperatureCelsius =
                WeatherCalculator.Round(
                    data.Main.MaximumTemperature),

            MaximumTemperatureFahrenheit =
                WeatherCalculator.CelsiusToFahrenheit(
                    data.Main.MaximumTemperature),

            DewPointCelsius = dewPointCelsius,

            DewPointFahrenheit =
                WeatherCalculator.CelsiusToFahrenheit(
                    dewPointCelsius),

            Humidity = data.Main.Humidity,
            PressureHpa = data.Main.Pressure,
            VisibilityMeters = data.Visibility,

            WindSpeedMetersPerSecond = data.Wind.Speed,

            Sky = condition?.Main ?? string.Empty,
            Description = condition?.Description ?? string.Empty,

            ObservedAtUtc =
                DateTimeOffset.FromUnixTimeSeconds(
                    data.Timestamp)
        };
    }

    private sealed class OpenWeatherResponse
    {
        [JsonPropertyName("main")]
        public MainData Main { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<WeatherCondition> Weather { get; set; } = new();

        [JsonPropertyName("wind")]
        public WindData Wind { get; set; } = new();

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }

        [JsonPropertyName("dt")]
        public long Timestamp { get; set; }
    }

    private sealed class MainData
    {
        [JsonPropertyName("temp")]
        public double Temperature { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("temp_min")]
        public double MinimumTemperature { get; set; }

        [JsonPropertyName("temp_max")]
        public double MaximumTemperature { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }
    }

    private sealed class WeatherCondition
    {
        [JsonPropertyName("main")]
        public string Main { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    private sealed class WindData
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}