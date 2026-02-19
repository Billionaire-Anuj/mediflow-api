namespace Mediflow.Application.DTOs.Appointments.Medications;

public class CreateAppointmentMedicationDrugsDto
{
    public Guid MedicineId { get; set; }

    public string Dose { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    public string? Instructions { get; set; }
}