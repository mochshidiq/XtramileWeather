using MediatR;
using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Common.Interfaces;

namespace XtramileWeather.Application.FavoriteCities
    .Commands.DeleteFavoriteCity;

public sealed class DeleteFavoriteCityCommandHandler
    : IRequestHandler<DeleteFavoriteCityCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteFavoriteCityCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteFavoriteCityCommand request,
        CancellationToken cancellationToken)
    {
        var favorite = await _context.FavoriteCities
            .SingleOrDefaultAsync(
                favorite => favorite.Id == request.FavoriteId,
                cancellationToken);

        if (favorite is null)
        {
            return false;
        }

        _context.FavoriteCities.Remove(favorite);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}