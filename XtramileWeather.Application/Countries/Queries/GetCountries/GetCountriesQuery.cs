using MediatR;

namespace XtramileWeather.Application.Countries.Queries.GetCountries;

public sealed record GetCountriesQuery
    : IRequest<IReadOnlyList<CountryDto>>;