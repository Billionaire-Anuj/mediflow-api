namespace Mediflow.Application.DTOs.Appointments.Diagnostics;

public class CreateAppointmentDiagnosticsDto
{
    public string Notes { get; set; } = string.Empty;

    public List<CreateAppointmentDiagnosticTestsDto> DiagnosticTests { get; set; } = new();
}