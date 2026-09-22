using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Country> Countries { get; }
    DbSet<City> Cities { get; }
    DbSet<FavoriteCity> FavoriteCities { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}