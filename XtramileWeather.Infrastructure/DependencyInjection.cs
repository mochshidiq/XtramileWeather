using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using XtramileWeather.Infrastructure.Persistence;
using XtramileWeather.Application.Common.Interfaces;

namespace XtramileWeather.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IAppDbContext>(serviceProvider =>
    serviceProvider.GetRequiredService<AppDbContext>());

        return services;
    }
}