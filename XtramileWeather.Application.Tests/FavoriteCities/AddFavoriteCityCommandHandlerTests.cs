using System.Diagnostics.Metrics;
using XtramileWeather.Application.FavoriteCities.Commands.AddFavoriteCity;
using XtramileWeather.Application.Tests.Common;
using XtramileWeather.Domain.Entities;
using Xunit;

namespace XtramileWeather.Application.Tests.FavoriteCities;

public class AddFavoriteCityCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddFavoriteCity_WhenCityExists()
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

        context.Countries.Add(country);
        context.Cities.Add(city);

        await context.SaveChangesAsync();

        var handler = new AddFavoriteCityCommandHandler(context);

        var result = await handler.Handle(
            new AddFavoriteCityCommand(city.Id),
            CancellationToken.None);

        Assert.Equal(AddFavoriteCityStatus.Created, result.Status);
        Assert.NotNull(result.Favorite);
        Assert.Equal(city.Id, result.Favorite.CityId);
        Assert.Single(context.FavoriteCities);
    }

    [Fact]
    public async Task Handle_ShouldRejectDuplicateFavoriteCity()
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

        context.Countries.Add(country);
        context.Cities.Add(city);

        context.FavoriteCities.Add(new FavoriteCity
        {
            Id = 1,
            CityId = city.Id,
            City = city,
            CreatedAtUtc = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var handler = new AddFavoriteCityCommandHandler(context);

        var result = await handler.Handle(
            new AddFavoriteCityCommand(city.Id),
            CancellationToken.None);

        Assert.Equal(
            AddFavoriteCityStatus.AlreadyExists,
            result.Status);

        Assert.Single(context.FavoriteCities);
    }
}