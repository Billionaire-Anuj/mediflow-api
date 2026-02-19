using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.DiagnosticTypes;

namespace Mediflow.Application.Interfaces.Services;

public interface IDiagnosticTypeService : ITransientService
{
    List<DiagnosticTypeDto> GetAllDiagnosticTypes(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null
    );

    List<DiagnosticTypeDto> GetAllDiagnosticTypes(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null
    );

    DiagnosticTypeDto GetDiagnosticTypeById(Guid diagnosticTypeId);

    void CreateDiagnosticType(CreateDiagnosticTypeDto diagnosticType);

    void UpdateDiagnosticType(Guid diagnosticTypeId, UpdateDiagnosticTypeDto diagnosticType);
    
    void ActivateDeactivateDiagnosticType(Guid diagnosticTypeId);
}