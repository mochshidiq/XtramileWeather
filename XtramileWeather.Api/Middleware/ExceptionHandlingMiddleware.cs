using XtramileWeather.Application.Common.Exceptions;

namespace XtramileWeather.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ExternalWeatherException exception)
        {
            _logger.LogError(
                exception,
                "Weather provider failed with status {ProviderStatusCode}.",
                exception.ProviderStatusCode);

            await WriteErrorAsync(
                context,
                StatusCodes.Status502BadGateway,
                "Weather service unavailable",
                "Weather data could not be retrieved. " +
                "Please try again later.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unexpected application error occurred.");

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Unexpected application error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            title,
            detail
        });
    }
}