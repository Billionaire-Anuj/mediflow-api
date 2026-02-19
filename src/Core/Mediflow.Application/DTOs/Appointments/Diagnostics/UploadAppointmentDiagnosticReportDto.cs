using Microsoft.AspNetCore.Http;

namespace Mediflow.Application.DTOs.Appointments.Diagnostics;

public class UploadAppointmentDiagnosticReportDto
{
    public Guid AppointmentDiagnosticTestId { get; set; }

    public IFormFile Report { get; set; } = null!;
}
