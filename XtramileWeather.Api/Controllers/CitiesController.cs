using MediatR;
using Microsoft.AspNetCore.Mvc;
using XtramileWeather.Application.Cities;
using XtramileWeather.Application.Cities.Queries.GetCities;

namespace XtramileWeather.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CitiesController : ControllerBase
{
    private readonly ISender _sender;

    public CitiesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<CityDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CityDto>>> GetAll(
        [FromQuery] int? countryId,
        CancellationToken cancellationToken)
    {
        var cities = await _sender.Send(
            new GetCitiesQuery(countryId),
            cancellationToken);

        return Ok(cities);
    }
}