using MediatR;
using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Common.Interfaces;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Application.FavoriteCities
    .Commands.AddFavoriteCity;

public sealed class AddFavoriteCityCommandHandler
    : IRequestHandler<
        AddFavoriteCityCommand,
        AddFavoriteCityResult>
{
    private readonly IAppDbContext _context;

    public AddFavoriteCityCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<AddFavoriteCityResult> Handle(
        AddFavoriteCityCommand request,
        CancellationToken cancellationToken)
    {
        var city = await _context.Cities
            .AsNoTracking()
            .Where(city => city.Id == request.CityId)
            .Select(city => new
            {
                city.Id,
                city.Name,
                CountryCode = city.Country.Code
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (city is null)
        {
            return new AddFavoriteCityResult(
                AddFavoriteCityStatus.CityNotFound,
                null);
        }

        var alreadyExists = await _context.FavoriteCities
            .AnyAsync(
                favorite => favorite.CityId == request.CityId,
                cancellationToken);

        if (alreadyExists)
        {
            return new AddFavoriteCityResult(
                AddFavoriteCityStatus.AlreadyExists,
                null);
        }

        var favorite = new FavoriteCity
        {
            CityId = city.Id,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.FavoriteCities.Add(favorite);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new FavoriteCityDto
        {
            Id = favorite.Id,
            CityId = city.Id,
            CityName = city.Name,
            CountryCode = city.CountryCode,
            CreatedAtUtc = favorite.CreatedAtUtc
        };

        return new AddFavoriteCityResult(
            AddFavoriteCityStatus.Created,
            dto);
    }
}