using MediatR;
using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Common.Interfaces;

namespace XtramileWeather.Application.Weather.Queries.GetCurrentWeather;

public sealed class GetCurrentWeatherQueryHandler
    : IRequestHandler<GetCurrentWeatherQuery, WeatherDto?>
{
    private readonly IAppDbContext _context;
    private readonly IWeatherService _weatherService;

    public GetCurrentWeatherQueryHandler(
        IAppDbContext context,
        IWeatherService weatherService)
    {
        _context = context;
        _weatherService = weatherService;
    }

    public async Task<WeatherDto?> Handle(
        GetCurrentWeatherQuery request,
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
            return null;
        }

        return await _weatherService.GetCurrentAsync(
            city.Id,
            city.Name,
            city.CountryCode,
            cancellationToken);
    }
}