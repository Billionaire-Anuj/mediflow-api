using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.DiagnosticTests;

namespace Mediflow.Application.Interfaces.Services;

public interface IDiagnosticTestService : ITransientService
{
    List<DiagnosticTestDto> GetAllDiagnosticTests(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? diagnosticTypeIds = null,
        string? title = null,
        string? description = null,
        string? specimen = null
    );

    List<DiagnosticTestDto> GetAllDiagnosticTests(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? diagnosticTypeIds = null,
        string? title = null,
        string? description = null,
        string? specimen = null
    );

    DiagnosticTestDto GetDiagnosticTestById(Guid diagnosticTestId);

    void CreateDiagnosticTest(CreateDiagnosticTestDto diagnosticTest);

    void UpdateDiagnosticTest(Guid diagnosticTestId, UpdateDiagnosticTestDto diagnosticTest);
    
    void ActivateDeactivateDiagnosticTest(Guid diagnosticTestId);
}