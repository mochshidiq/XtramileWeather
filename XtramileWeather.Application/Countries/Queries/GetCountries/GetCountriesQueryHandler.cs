using MediatR;
using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Common.Interfaces;

namespace XtramileWeather.Application.Countries.Queries.GetCountries;

public sealed class GetCurrentWeatherQueryHandler
    : IRequestHandler<GetCurrentWeatherQuery, IReadOnlyList<CountryDto>>
{
    private readonly IAppDbContext _context;

    public GetCurrentWeatherQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CountryDto>> Handle(
        GetCurrentWeatherQuery request,
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