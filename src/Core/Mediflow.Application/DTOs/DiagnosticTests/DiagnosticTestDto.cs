using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.DiagnosticTypes;

namespace Mediflow.Application.DTOs.DiagnosticTests;

public class DiagnosticTestDto : BaseDto
{
    public DiagnosticTypeDto DiagnosticType { get; set; } = new();

    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public string Specimen { get; set; } = string.Empty;
}