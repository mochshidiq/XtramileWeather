using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using XtramileWeather.Application.Common.Interfaces;
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
            throw new InvalidOperationException(
                "OpenWeather API key is not configured.");
        }

        var location = Uri.EscapeDataString(
            $"{cityName},{countryCode}");

        var url =
            $"weather?q={location}" +
            $"&appid={Uri.EscapeDataString(_options.ApiKey)}" +
            "&units=metric";

        var response = await _httpClient.GetAsync(
            url,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var data =
            await response.Content.ReadFromJsonAsync<OpenWeatherResponse>(
                cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException(
                "OpenWeather returned an empty response.");

        return new WeatherDto
        {
            CityId = cityId,
            CityName = cityName,
            CountryCode = countryCode,
            TemperatureCelsius = data.Main.Temperature,
            FeelsLikeCelsius = data.Main.FeelsLike,
            MinimumTemperatureCelsius = data.Main.MinimumTemperature,
            MaximumTemperatureCelsius = data.Main.MaximumTemperature,
            Humidity = data.Main.Humidity,
            WindSpeedMetersPerSecond = data.Wind.Speed,
            Description =
                data.Weather.FirstOrDefault()?.Description ?? string.Empty,
            ObservedAtUtc =
                DateTimeOffset.FromUnixTimeSeconds(data.Timestamp)
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
    }

    private sealed class WeatherCondition
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    private sealed class WindData
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}