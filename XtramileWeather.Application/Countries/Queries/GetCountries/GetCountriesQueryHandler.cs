using MediatR;
using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Common.Interfaces;
using XtramileWeather.Application.Countries.Queries.GetCountries;

namespace XtramileWeather.Application.Countries.Queries.GetCountries;

public sealed class GetCountriesQueryHandler
    : IRequestHandler<GetCountriesQuery, IReadOnlyList<CountryDto>>
{
    private readonly IAppDbContext _context;

    public GetCountriesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CountryDto>> Handle(
        GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Countries
            .AsNoTracking()
            .OrderBy(country => country.Name)
            .Select(country => new CountryDto
            {
                Id = country.Id,
                Code = country.Code,
                Name = country.Name
            })
            .ToListAsync(cancellationToken);
    }
}