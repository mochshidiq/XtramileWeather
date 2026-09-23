using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using XtramileWeather.Application.FavoriteCities.Commands.DeleteFavoriteCity;
using XtramileWeather.Application.Tests.Common;
using XtramileWeather.Domain.Entities;
using Xunit;

namespace XtramileWeather.Application.Tests.FavoriteCities;

public class DeleteFavoriteCityCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteFavoriteCity_WhenFavoriteExists()
    {
        await using var context = TestDbContextFactory.Create();

        var country = new Country
        {
            Id = 1,
            Code = "ID",
            Name = "Indonesia"
        };

        var city = new City
        {
            Id = 1,
            Name = "Jakarta",
            CountryId = country.Id,
            Country = country
        };

        var favorite = new FavoriteCity
        {
            Id = 10,
            CityId = city.Id,
            City = city,
            CreatedAtUtc = DateTime.UtcNow
        };

        context.Countries.Add(country);
        context.Cities.Add(city);
        context.FavoriteCities.Add(favorite);

        await context.SaveChangesAsync();

        var handler = new DeleteFavoriteCityCommandHandler(context);

        var result = await handler.Handle(
            new DeleteFavoriteCityCommand(favorite.Id),
            CancellationToken.None);

        Assert.True(result);

        var favoriteStillExists =
            await context.FavoriteCities.AnyAsync(
                item => item.Id == favorite.Id);

        Assert.False(favoriteStillExists);
    }
}