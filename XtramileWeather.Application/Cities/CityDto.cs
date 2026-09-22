namespace XtramileWeather.Application.Cities;

public sealed class CityDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CountryId { get; init; }
    public string CountryCode { get; init; } = string.Empty;
}