namespace XtramileWeather.Application.Common.Utilities;

public static class WeatherCalculator
{
    private const double MagnusA = 17.625;
    private const double MagnusB = 243.04;

    public static double CelsiusToFahrenheit(double celsius)
    {
        return Round((celsius * 9 / 5) + 32);
    }

    public static double CalculateDewPointCelsius(
        double temperatureCelsius,
        int humidity)
    {
        var safeHumidity = Math.Clamp(humidity, 1, 100);

        var gamma =
            Math.Log(safeHumidity / 100.0) +
            (MagnusA * temperatureCelsius) /
            (MagnusB + temperatureCelsius);

        var dewPoint =
            (MagnusB * gamma) /
            (MagnusA - gamma);

        return Round(dewPoint);
    }

    public static double Round(double value)
    {
        return Math.Round(
            value,
            1,
            MidpointRounding.AwayFromZero);
    }
}