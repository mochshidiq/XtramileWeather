using MediatR;

namespace XtramileWeather.Application.Countries.Queries.GetCountries;

public sealed record GetCurrentWeatherQuery
    : IRequest<IReadOnlyList<CountryDto>>;