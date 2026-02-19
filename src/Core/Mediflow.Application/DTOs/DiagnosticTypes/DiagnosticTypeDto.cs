using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.DiagnosticTypes;

public class DiagnosticTypeDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}