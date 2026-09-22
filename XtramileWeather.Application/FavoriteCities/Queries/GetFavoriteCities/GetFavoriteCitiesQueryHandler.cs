using MediatR;
using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Common.Interfaces;

namespace XtramileWeather.Application.FavoriteCities
    .Queries.GetFavoriteCities;

public sealed class GetFavoriteCitiesQueryHandler
    : IRequestHandler<
        GetFavoriteCitiesQuery,
        IReadOnlyList<FavoriteCityDto>>
{
    private readonly IAppDbContext _context;

    public GetFavoriteCitiesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FavoriteCityDto>> Handle(
        GetFavoriteCitiesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.FavoriteCities
            .AsNoTracking()
            .OrderBy(favorite => favorite.City.Name)
            .Select(favorite => new FavoriteCityDto
            {
                Id = favorite.Id,
                CityId = favorite.CityId,
                CityName = favorite.City.Name,
                CountryCode = favorite.City.Country.Code,
                CreatedAtUtc = favorite.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}