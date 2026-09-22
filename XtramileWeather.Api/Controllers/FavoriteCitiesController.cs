using MediatR;
using Microsoft.AspNetCore.Mvc;
using XtramileWeather.Application.FavoriteCities;
using XtramileWeather.Application.FavoriteCities.Commands.AddFavoriteCity;
using XtramileWeather.Application.FavoriteCities.Commands.DeleteFavoriteCity;
using XtramileWeather.Application.FavoriteCities.Queries.GetFavoriteCities;

namespace XtramileWeather.Api.Controllers;

[ApiController]
[Route("api/favorite-cities")]
public sealed class FavoriteCitiesController : ControllerBase
{
    private readonly ISender _sender;

    public FavoriteCitiesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FavoriteCityDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var favorites = await _sender.Send(
            new GetFavoriteCitiesQuery(),
            cancellationToken);

        return Ok(favorites);
    }

    [HttpPost("{cityId:int}")]
    public async Task<IActionResult> Add(
        int cityId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AddFavoriteCityCommand(cityId),
            cancellationToken);

        return result.Status switch
        {
            AddFavoriteCityStatus.Created =>
                StatusCode(StatusCodes.Status201Created, result.Favorite),

            AddFavoriteCityStatus.CityNotFound =>
                NotFound(new
                {
                    message = $"City with ID {cityId} was not found."
                }),

            AddFavoriteCityStatus.AlreadyExists =>
                Conflict(new
                {
                    message = "City is already marked as favorite."
                }),

            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{favoriteId:int}")]
    public async Task<IActionResult> Delete(
        int favoriteId,
        CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(
            new DeleteFavoriteCityCommand(favoriteId),
            cancellationToken);

        return deleted
            ? NoContent()
            : NotFound(new
            {
                message =
                    $"Favorite with ID {favoriteId} was not found."
            });
    }
}