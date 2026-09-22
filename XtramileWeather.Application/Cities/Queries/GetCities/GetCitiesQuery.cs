using MediatR;

namespace XtramileWeather.Application.Cities.Queries.GetCities;

public sealed record GetCitiesQuery(int? CountryId)
    : IRequest<IReadOnlyList<CityDto>>;