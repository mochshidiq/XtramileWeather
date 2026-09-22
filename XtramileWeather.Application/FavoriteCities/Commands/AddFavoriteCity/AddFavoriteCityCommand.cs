using MediatR;

namespace XtramileWeather.Application.FavoriteCities
    .Commands.AddFavoriteCity;

public sealed record AddFavoriteCityCommand(int CityId)
    : IRequest<AddFavoriteCityResult>;

public enum AddFavoriteCityStatus
{
    Created,
    CityNotFound,
    AlreadyExists
}

public sealed record AddFavoriteCityResult(
    AddFavoriteCityStatus Status,
    FavoriteCityDto? Favorite);