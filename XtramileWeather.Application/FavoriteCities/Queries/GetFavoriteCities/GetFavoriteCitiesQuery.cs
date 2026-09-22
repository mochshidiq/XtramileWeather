using MediatR;

namespace XtramileWeather.Application.FavoriteCities
    .Queries.GetFavoriteCities;

public sealed record GetFavoriteCitiesQuery
    : IRequest<IReadOnlyList<FavoriteCityDto>>;