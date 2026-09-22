namespace XtramileWeather.Application.Common.Exceptions;

public sealed class ExternalWeatherException : Exception
{
    public int? ProviderStatusCode { get; }

    public ExternalWeatherException(
        string message,
        int? providerStatusCode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ProviderStatusCode = providerStatusCode;
    }
}