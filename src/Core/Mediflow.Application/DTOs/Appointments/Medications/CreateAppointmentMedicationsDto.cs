namespace Mediflow.Application.DTOs.Appointments.Medications;

public class CreateAppointmentMedicationsDto
{
    public string Notes { get; set; } = string.Empty;

    public List<CreateAppointmentMedicationDrugsDto> Drugs { get; set; } = new();
}