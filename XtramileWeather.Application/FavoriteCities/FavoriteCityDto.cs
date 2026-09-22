namespace XtramileWeather.Application.FavoriteCities;

public sealed class FavoriteCityDto
{
    public int Id { get; init; }
    public int CityId { get; init; }
    public string CityName { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}