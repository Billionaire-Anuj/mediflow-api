using Mediflow.Application.Interfaces.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace Mediflow.Infrastructure.Dependency;

public static class MigrationService
{
    public static void AddDataSeedService(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
    
        using var scope = serviceProvider.CreateScope();
    
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        #region Roles
        dbInitializer.InitializeRolesData();
        #endregion

        #region Resources
        dbInitializer.InitializeResourcesData();
        #endregion

        #region Specializations
        dbInitializer.InitializeSpecializationsData();
        #endregion

        #region Permissions
        dbInitializer.InitializePermissionsData();
        #endregion

        #region Administrators
        dbInitializer.InitializeAdministratorsData();
        #endregion
    }
}
