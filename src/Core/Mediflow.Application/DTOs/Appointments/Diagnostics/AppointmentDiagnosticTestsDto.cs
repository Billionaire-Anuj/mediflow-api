using Mediflow.Application.DTOs.Assets;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.DiagnosticTests;

namespace Mediflow.Application.DTOs.Appointments.Diagnostics;

public class AppointmentDiagnosticTestsDto : BaseDto
{
    public DiagnosticTestDto DiagnosticTest { get; set; } = new();

    public AppointmentDiagnosticTestResultDto? Result { get; set; }

    public AssetDto? Report { get; set; }
}