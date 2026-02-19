namespace Mediflow.Application.DTOs.Appointments.Diagnostics;

public class UpdateAppointmentDiagnosticTestResultDto
{
    public Guid AppointmentDiagnosticTestId { get; set; }

    public string Value { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public string UpperRange { get; set; } = string.Empty;

    public string LowerRange { get; set; } = string.Empty;

    public string Interpretation { get; set; } = string.Empty;
}
