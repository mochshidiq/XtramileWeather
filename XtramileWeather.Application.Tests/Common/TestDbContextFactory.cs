using Microsoft.EntityFrameworkCore;
using System;
using XtramileWeather.Infrastructure.Persistence;

namespace XtramileWeather.Application.Tests.Common;

public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }
}