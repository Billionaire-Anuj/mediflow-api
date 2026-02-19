using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.MedicationTypes;

namespace Mediflow.Application.DTOs.Medicines;

public class MedicineDto : BaseDto
{
    public MedicationTypeDto MedicationType { get; set; } = new();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Format { get; set; } = string.Empty;
}
