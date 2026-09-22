using XtramileWeather.Application.Weather;

namespace XtramileWeather.Application.Common.Interfaces;

public interface IWeatherService
{
    Task<WeatherDto> GetCurrentAsync(
        int cityId,
        string cityName,
        string countryCode,
        CancellationToken cancellationToken);
}