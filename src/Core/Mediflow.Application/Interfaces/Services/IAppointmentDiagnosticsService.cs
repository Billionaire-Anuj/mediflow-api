using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Appointments.Diagnostics;

namespace Mediflow.Application.Interfaces.Services;

public interface IAppointmentDiagnosticsService : ITransientService
{
    List<AppointmentDiagnosticsDto> GetAllAppointmentDiagnostics(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? appointmentId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? labTechnicianId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        DiagnosticStatus[]? statuses = null
    );

    List<AppointmentDiagnosticsDto> GetAllAppointmentDiagnostics(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? appointmentId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? labTechnicianId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        DiagnosticStatus[]? statuses = null
    );

    AppointmentDiagnosticsDto GetAppointmentDiagnosticsById(Guid appointmentDiagnosticsId);

    void AssignLabTechnician(Guid appointmentDiagnosticsId);

    void UploadDiagnosticReport(Guid appointmentDiagnosticTestId, UploadAppointmentDiagnosticReportDto report);

    void SubmitDiagnosticTestResult(Guid appointmentDiagnosticTestId, UpdateAppointmentDiagnosticTestResultDto result);
}
