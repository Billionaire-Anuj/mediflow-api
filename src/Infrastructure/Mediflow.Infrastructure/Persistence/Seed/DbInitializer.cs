using Microsoft.Extensions.Logging;
using Mediflow.Application.Interfaces.Seed;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class DbInitializer(ILogger<DbInitializer> logger, IEnumerable<IDataSeeder> seeders) : IDbInitializer
{
    public void InitializeRolesData()
    {
        InitializeInternal(static x => x is RoleSeeder);
    }

    public void InitializeResourcesData()
    {
        InitializeInternal(static x => x is ResourcesSeeder);
    }

    public void InitializePermissionsData()
    {
        // Please comment on the following line after the first round of permission seeding.
        // InitializeInternal(static x => x is PermissionsSeeder);
    }

    public void InitializeAdministratorsData()
    {
        InitializeInternal(static x => x is AdministratorsSeeder);
    }

    public void InitializeSpecializationsData()
    {
        InitializeInternal(static x => x is SpecializationSeeder);
    }

    private void InitializeInternal(Func<IDataSeeder, bool> predicate)
    {
        var dataSeeders = seeders.Where(predicate).OrderBy(x => x.Order).ToList();

        if (dataSeeders.Count == 0)
        {
            logger.LogWarning("No seeders found for the specified predicate.");
            return;
        }

        foreach (var dataSeeder in dataSeeders)
        {
            logger.LogInformation("Running seeder {SeederName} (Order {Order})", dataSeeder.GetType().Name, dataSeeder.Order);

            dataSeeder.Seed();
        }
    }
}
