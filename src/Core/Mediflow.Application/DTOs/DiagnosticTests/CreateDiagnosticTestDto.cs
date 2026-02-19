namespace Mediflow.Application.DTOs.DiagnosticTests;

public class CreateDiagnosticTestDto
{
    public Guid DiagnosticTypeId { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public string Specimen { get; set; } = string.Empty;
}