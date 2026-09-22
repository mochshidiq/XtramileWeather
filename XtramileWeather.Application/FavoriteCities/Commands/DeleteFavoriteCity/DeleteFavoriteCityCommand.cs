using MediatR;

namespace XtramileWeather.Application.FavoriteCities
    .Commands.DeleteFavoriteCity;

public sealed record DeleteFavoriteCityCommand(int FavoriteId)
    : IRequest<bool>;