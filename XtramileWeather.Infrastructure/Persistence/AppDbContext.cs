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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Code)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(x => x.Code)
                    .IsUnique();
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(x => x.Country)
                    .WithMany()
                    .HasForeignKey(x => x.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.CountryId, x.Name })
                    .IsUnique();
            });

            modelBuilder.Entity<FavoriteCity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.City)
                    .WithMany()
                    .HasForeignKey(x => x.CityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.CityId)
                    .IsUnique();

                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();
            });
        }
    }
}
