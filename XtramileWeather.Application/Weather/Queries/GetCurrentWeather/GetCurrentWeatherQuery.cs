using MediatR;

namespace XtramileWeather.Application.Weather.Queries.GetCurrentWeather;

public sealed record GetCurrentWeatherQuery(int CityId)
    : IRequest<WeatherDto?>;