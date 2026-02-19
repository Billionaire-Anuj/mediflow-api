namespace Mediflow.Application.DTOs.Medicines;

public class CreateMedicineDto
{
    public Guid MedicationTypeId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Format { get; set; } = string.Empty;
}
