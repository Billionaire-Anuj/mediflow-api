using Mediflow.Application.DTOs.Appointments.Diagnostics;
using Mediflow.Application.DTOs.Appointments.Medications;

namespace Mediflow.Application.DTOs.Appointments;

public class ConsultAppointmentDto
{
    public Guid AppointmentId { get; set; }

    public string Diagnosis { get; set; } = string.Empty;

    public string Treatment { get; set; } = string.Empty;

    public string Prescriptions { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public List<CreateAppointmentDiagnosticsDto> Diagnostics { get; set; } = new();

    public List<CreateAppointmentMedicationsDto> Medications { get; set; } = new();
}