using Mediflow.Application.DTOs.Medicines;
using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Appointments.Medications;

public class AppointmentMedicationDrugsDto : BaseDto
{
    public MedicineDto Medicine { get; set; } = new();

    public string Dose { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    public string? Instructions { get; set; }
}