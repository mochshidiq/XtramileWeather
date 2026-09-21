using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using XtramileWeather.Infrastructure.Persistence;

namespace XtramileWeather.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}