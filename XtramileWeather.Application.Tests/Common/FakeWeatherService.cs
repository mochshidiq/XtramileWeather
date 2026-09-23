using XtramileWeather.Application.Common.Interfaces;
using XtramileWeather.Application.Weather;


namespace XtramileWeather.Application.Tests.Common;

public sealed class FakeWeatherService : IWeatherService
{
    public int CallCount { get; private set; }

    public int? ReceivedCityId { get; private set; }

    public string? ReceivedCityName { get; private set; }

    public string? ReceivedCountryCode { get; private set; }

    public WeatherDto Response { get; set; } = null!;

    public Task<WeatherDto> GetCurrentAsync(
        int cityId,
        string cityName,
        string countryCode,
        CancellationToken cancellationToken)
    {
        CallCount++;
        ReceivedCityId = cityId;
        ReceivedCityName = cityName;
        ReceivedCountryCode = countryCode;

        return Task.FromResult(Response);
    }
}