using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Appointments.Diagnostics;

public class AppointmentDiagnosticTestResultDto : BaseDto
{
    public string Value { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public string UpperRange { get; set; } = string.Empty;

    public string LowerRange { get; set; } = string.Empty;

    public string Interpretation { get; set; } = string.Empty;
}