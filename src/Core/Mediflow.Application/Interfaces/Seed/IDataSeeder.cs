using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Seed;

public interface IDataSeeder : IScopedService
{
    /// <summary>
    /// Order in which this seeder should execute.
    /// </summary>
    int Order { get; }

    void Seed();
}
