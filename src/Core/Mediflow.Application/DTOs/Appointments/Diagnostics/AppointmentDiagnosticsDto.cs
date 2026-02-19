using Mediflow.Domain.Common.Enum;
using Mediflow.Application.DTOs.Users;
using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Appointments.Diagnostics;

public class AppointmentDiagnosticsDto : BaseDto
{
    public UserDto? LabTechnician { get; set; }

    public string Notes { get; set; } = string.Empty;

    public DiagnosticStatus Status { get; set; }

    public DateTime? CompletedDate { get; set; }

    public List<AppointmentDiagnosticTestsDto> DiagnosticTests { get; set; } = new();
}