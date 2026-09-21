using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries => Set<Country>();

        public DbSet<City> Cities => Set<City>();

        public DbSet<FavoriteCity> FavoriteCities => Set<FavoriteCity>();
    }
}
