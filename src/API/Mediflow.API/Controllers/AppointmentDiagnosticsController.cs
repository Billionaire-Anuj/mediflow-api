using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.Domain.Common.Enum;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Appointments;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Appointments.Diagnostics;

namespace Mediflow.API.Controllers;

public class AppointmentDiagnosticsController(IAppointmentDiagnosticsService appointmentDiagnosticsService) : BaseController<AppointmentDiagnosticsController>
{
    [HttpGet]
    [Documentation("GetAllAppointmentDiagnostics", "Retrieve all paginated appointment diagnostics with appointment details.")]
    public CollectionDto<AppointmentDto> GetAllAppointmentDiagnostics(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] Guid? appointmentId = null,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? labTechnicianId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] DiagnosticStatus[]? statuses = null)
    {
        var result = appointmentDiagnosticsService.GetAllAppointmentDiagnostics(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            appointmentId,
            doctorId,
            patientId,
            labTechnicianId,
            startDate,
            endDate,
            statuses);

        return new CollectionDto<AppointmentDto>(
            (int)HttpStatusCode.OK,
            "The appointment diagnostics have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllAppointmentDiagnosticsList", "Retrieve all non-paginated appointment diagnostics with appointment details.")]
    public ResponseDto<List<AppointmentDto>> GetAllAppointmentDiagnostics(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] Guid? appointmentId = null,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? labTechnicianId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] DiagnosticStatus[]? statuses = null)
    {
        var result = appointmentDiagnosticsService.GetAllAppointmentDiagnostics(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            appointmentId,
            doctorId,
            patientId,
            labTechnicianId,
            startDate,
            endDate,
            statuses);

        return new ResponseDto<List<AppointmentDto>>(
            (int)HttpStatusCode.OK,
            "The appointment diagnostics have been successfully retrieved.",
            result);
    }

    [HttpGet("{appointmentDiagnosticsId:guid}")]
    [Documentation("GetAppointmentDiagnosticsById", "Retrieve the respective appointment diagnostics via its identifier in the system.")]
    public ResponseDto<AppointmentDiagnosticsDto> GetAppointmentDiagnosticsById([FromRoute] Guid appointmentDiagnosticsId)
    {
        var result = appointmentDiagnosticsService.GetAppointmentDiagnosticsById(appointmentDiagnosticsId);

        return new ResponseDto<AppointmentDiagnosticsDto>(
            (int)HttpStatusCode.OK,
            "Appointment diagnostics successfully fetched.",
            result);
    }

    [HttpPatch("{appointmentDiagnosticsId:guid}/assign")]
    [Documentation("AssignLabTechnician", "Assigns the logged in lab technician to the diagnostics.")]
    public ResponseDto<bool> AssignLabTechnician([FromRoute] Guid appointmentDiagnosticsId)
    {
        appointmentDiagnosticsService.AssignLabTechnician(appointmentDiagnosticsId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Lab technician successfully assigned.",
            true);
    }

    [HttpPost("{appointmentDiagnosticTestId:guid}/report")]
    [Documentation("UploadDiagnosticReport", "Uploads a diagnostic report for the test.")]
    public ResponseDto<bool> UploadDiagnosticReport([FromRoute] Guid appointmentDiagnosticTestId, [FromForm] UploadAppointmentDiagnosticReportDto report)
    {
        appointmentDiagnosticsService.UploadDiagnosticReport(appointmentDiagnosticTestId, report);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Diagnostic report successfully uploaded.",
            true);
    }

    [HttpPost("{appointmentDiagnosticTestId:guid}/result")]
    [Documentation("SubmitDiagnosticTestResult", "Submits diagnostic test results for the test.")]
    public ResponseDto<bool> SubmitDiagnosticTestResult([FromRoute] Guid appointmentDiagnosticTestId, [FromBody] UpdateAppointmentDiagnosticTestResultDto result)
    {
        appointmentDiagnosticsService.SubmitDiagnosticTestResult(appointmentDiagnosticTestId, result);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Diagnostic test result successfully submitted.",
            true);
    }
}
