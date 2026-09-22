using MediatR;
using Microsoft.AspNetCore.Mvc;
using XtramileWeather.Application.Countries;
using XtramileWeather.Application.Countries.Queries.GetCountries;

namespace XtramileWeather.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CountriesController : ControllerBase
{
    private readonly ISender _sender;

    public CountriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<CountryDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CountryDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var countries = await _sender.Send(
            new GetCurrentWeatherQuery(),
            cancellationToken);

        return Ok(countries);
    }
}