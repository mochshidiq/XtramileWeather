using System.Diagnostics.Metrics;
using XtramileWeather.Application.Cities.Queries.GetCities;
using XtramileWeather.Application.Tests.Common;
using XtramileWeather.Domain.Entities;
using Xunit;

namespace XtramileWeather.Application.Tests.Cities;

public class GetCitiesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCitiesFromSelectedCountry()
    {
        await using var context = TestDbContextFactory.Create();

        var indonesia = new Country
        {
            Id = 1,
            Code = "ID",
            Name = "Indonesia"
        };

        var australia = new Country
        {
            Id = 2,
            Code = "AU",
            Name = "Australia"
        };

        context.Countries.AddRange(indonesia, australia);

        context.Cities.AddRange(
            new City
            {
                Id = 1,
                Name = "Jakarta",
                CountryId = indonesia.Id,
                Country = indonesia
            },
            new City
            {
                Id = 2,
                Name = "Bandung",
                CountryId = indonesia.Id,
                Country = indonesia
            },
            new City
            {
                Id = 3,
                Name = "Sydney",
                CountryId = australia.Id,
                Country = australia
            });

        await context.SaveChangesAsync();

        var handler = new GetCitiesQueryHandler(context);

        var result = await handler.Handle(
            new GetCitiesQuery(indonesia.Id),
            CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, city =>
            Assert.Equal(indonesia.Id, city.CountryId));

        Assert.DoesNotContain(result, city =>
            city.Name == "Sydney");
    }
}