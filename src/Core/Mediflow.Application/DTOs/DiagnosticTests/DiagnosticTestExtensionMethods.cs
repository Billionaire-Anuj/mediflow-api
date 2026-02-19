using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.DiagnosticTypes;

namespace Mediflow.Application.DTOs.DiagnosticTests;

public static class DiagnosticTestExtensionMethods
{
    public static DiagnosticTestDto ToDiagnosticTestDto(this DiagnosticTest diagnosticTest)
    {
        return new DiagnosticTestDto
        {
            Id = diagnosticTest.Id,
            Title = diagnosticTest.Title,
            Specimen = diagnosticTest.Specimen,
            IsActive =  diagnosticTest.IsActive,
            Description = diagnosticTest.Description,
            DiagnosticType = (diagnosticTest.DiagnosticType ?? DiagnosticType.Default).ToDiagnosticTypeDto()
        };
    }
}