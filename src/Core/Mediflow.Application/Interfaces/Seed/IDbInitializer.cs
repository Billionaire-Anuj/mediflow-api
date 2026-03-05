using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Seed;

public interface IDbInitializer : IScopedService
{
    void InitializeRolesData();

    void InitializeResourcesData();

    void InitializeSpecializationsData();

    void InitializeDiagnosticTypesData();

    void InitializeDiagnosticTestsData();

    void InitializeMedicationTypesData();

    void InitializeMedicinesData();

    void InitializePermissionsData();

    void InitializeAdministratorsData();
}
