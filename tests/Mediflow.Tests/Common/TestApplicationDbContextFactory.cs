using Mediflow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Tests.Common;

internal static class TestApplicationDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}

