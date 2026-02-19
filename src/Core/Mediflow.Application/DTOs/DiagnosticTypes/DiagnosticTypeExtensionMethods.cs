using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.DiagnosticTypes;

public static class DiagnosticTypeExtensionMethods
{
    public static DiagnosticTypeDto ToDiagnosticTypeDto(this DiagnosticType diagnosticType)
    {
        return new DiagnosticTypeDto
        {
            Id = diagnosticType.Id,
            Title = diagnosticType.Title,
            IsActive =  diagnosticType.IsActive,
            Description = diagnosticType.Description
        };
    }
}