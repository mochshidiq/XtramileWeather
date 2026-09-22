using XtramileWeather.Infrastructure;
using XtramileWeather.Application.Countries.Queries.GetCountries;
using Microsoft.Extensions.Options;
using XtramileWeather.Application.Common.Interfaces;
using XtramileWeather.Infrastructure.Weather;
using XtramileWeather.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection is not configured.");

builder.Services.AddInfrastructure(connectionString);

builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(
        typeof(GetCurrentWeatherQuery).Assembly));

builder.Services.Configure<OpenWeatherOptions>(
    builder.Configuration.GetSection(
        OpenWeatherOptions.SectionName));

builder.Services.AddHttpClient<IWeatherService, OpenWeatherService>(
    (serviceProvider, httpClient) =>
    {
        var options = serviceProvider
            .GetRequiredService<IOptions<OpenWeatherOptions>>()
            .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.UseAuthorization();

app.MapControllers();

app.Run();
