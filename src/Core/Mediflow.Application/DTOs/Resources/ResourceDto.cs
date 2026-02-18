using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Resources;

public class ResourceDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}