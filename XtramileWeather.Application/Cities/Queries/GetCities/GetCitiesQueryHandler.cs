using MediatR;
using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Common.Interfaces;

namespace XtramileWeather.Application.Cities.Queries.GetCities;

public sealed class GetCitiesQueryHandler
    : IRequestHandler<GetCitiesQuery, IReadOnlyList<CityDto>>
{
    private readonly IAppDbContext _context;

    public GetCitiesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CityDto>> Handle(
        GetCitiesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Cities
            .AsNoTracking()
            .AsQueryable();

        if (request.CountryId.HasValue)
        {
            query = query.Where(city =>
                city.CountryId == request.CountryId.Value);
        }

        return await query
            .OrderBy(city => city.Name)
            .Select(city => new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                CountryId = city.CountryId,
                CountryCode = city.Country.Code
            })
            .ToListAsync(cancellationToken);
    }
}