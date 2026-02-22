using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Seed;

public interface IDbInitializer : IScopedService
{
    void InitializeRolesData();

    void InitializeResourcesData();

    void InitializeSpecializationsData();

    void InitializePermissionsData();

    void InitializeAdministratorsData();
}
