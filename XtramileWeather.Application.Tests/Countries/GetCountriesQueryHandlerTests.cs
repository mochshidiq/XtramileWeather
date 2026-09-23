using System.Diagnostics.Metrics;
using XtramileWeather.Application.Countries.Queries.GetCountries;
using XtramileWeather.Application.Tests.Common;
using XtramileWeather.Domain.Entities;
using Xunit;

namespace XtramileWeather.Application.Tests.Countries;

public class GetCountriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCountriesSortedByName()
    {
        await using var context = TestDbContextFactory.Create();

        context.Countries.AddRange(
            new Country
            {
                Id = 1,
                Code = "ID",
                Name = "Indonesia"
            },
            new Country
            {
                Id = 2,
                Code = "AU",
                Name = "Australia"
            });

        await context.SaveChangesAsync();

        var handler = new GetCountriesQueryHandler(context);

        var result = await handler.Handle(
            new GetCountriesQuery(),
            CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Australia", result[0].Name);
        Assert.Equal("Indonesia", result[1].Name);
    }
}