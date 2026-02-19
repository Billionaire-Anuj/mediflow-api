using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Specializations;

public class SpecializationDto : BaseDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
