using MediatR;
using Microsoft.AspNetCore.Mvc;
using XtramileWeather.Application.Weather;
using XtramileWeather.Application.Weather.Queries.GetCurrentWeather;

namespace XtramileWeather.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class WeatherController : ControllerBase
{
    private readonly ISender _sender;

    public WeatherController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{cityId:int}")]
    [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherDto>> GetCurrent(
        int cityId,
        CancellationToken cancellationToken)
    {
        var weather = await _sender.Send(
            new GetCurrentWeatherQuery(cityId),
            cancellationToken);

        if (weather is null)
        {
            return NotFound(new
            {
                message = $"City with ID {cityId} was not found."
            });
        }

        return Ok(weather);
    }
}