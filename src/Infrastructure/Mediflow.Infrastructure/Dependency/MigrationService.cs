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

        #region Diagnostic Types
        dbInitializer.InitializeDiagnosticTypesData();
        #endregion

        #region Diagnostic Tests
        dbInitializer.InitializeDiagnosticTestsData();
        #endregion

        #region Medication Types
        dbInitializer.InitializeMedicationTypesData();
        #endregion

        #region Medicines
        dbInitializer.InitializeMedicinesData();
        #endregion

        #region Permissions
        dbInitializer.InitializePermissionsData();
        #endregion

        #region Administrators
        dbInitializer.InitializeAdministratorsData();
        #endregion
    }
}
