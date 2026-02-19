using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.MedicationTypes;

public class MedicationTypeDto : BaseDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
