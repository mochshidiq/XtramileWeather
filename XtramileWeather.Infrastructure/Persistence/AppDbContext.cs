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

            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Code = "ID", Name = "Indonesia" },
                new Country { Id = 2, Code = "AU", Name = "Australia" },
                new Country { Id = 3, Code = "SG", Name = "Singapore" }
            );

            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Jakarta", CountryId = 1 },
                new City { Id = 2, Name = "Bandung", CountryId = 1 },
                new City { Id = 3, Name = "Surabaya", CountryId = 1 },

                new City { Id = 4, Name = "Sydney", CountryId = 2 },
                new City { Id = 5, Name = "Melbourne", CountryId = 2 },
                new City { Id = 6, Name = "Brisbane", CountryId = 2 },

                new City { Id = 7, Name = "Singapore", CountryId = 3 }
            );
        }
    }
}
