using System.Diagnostics.Metrics;
using XtramileWeather.Application.Tests.Common;
using XtramileWeather.Application.Weather.Queries.GetCurrentWeather;
using XtramileWeather.Domain.Entities;
using Xunit;

namespace XtramileWeather.Application.Tests.Weather;

public class GetCurrentWeatherQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCallWeatherServiceWithCorrectCity()
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

        var weatherService = new FakeWeatherService();

        var handler = new GetCurrentWeatherQueryHandler(
            context,
            weatherService);

        await handler.Handle(
            new GetCurrentWeatherQuery(city.Id),
            CancellationToken.None);

        Assert.Equal(1, weatherService.CallCount);
        Assert.Equal(city.Id, weatherService.ReceivedCityId);
        Assert.Equal("Jakarta", weatherService.ReceivedCityName);
        Assert.Equal("ID", weatherService.ReceivedCountryCode);
    }
}