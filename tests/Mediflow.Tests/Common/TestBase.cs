using Mediflow.Infrastructure.Persistence;

namespace Mediflow.Tests.Common;

public abstract class TestBase : IDisposable
{
    private ApplicationDbContext DbContext { get; } = TestApplicationDbContextFactory.Create();

    public void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
