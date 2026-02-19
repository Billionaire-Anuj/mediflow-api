using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.Domain.Common.Enum;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Appointments.Medications;

namespace Mediflow.API.Controllers;

public class AppointmentMedicationsController(IAppointmentMedicationsService appointmentMedicationsService) : BaseController<AppointmentMedicationsController>
{
    [HttpGet]
    [Documentation("GetAllAppointmentMedications", "Retrieve all paginated appointment medications in the system.")]
    public CollectionDto<AppointmentMedicationsDto> GetAllAppointmentMedications(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] Guid? appointmentId = null,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? pharmacistId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] DiagnosticStatus[]? statuses = null)
    {
        var result = appointmentMedicationsService.GetAllAppointmentMedications(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            appointmentId,
            doctorId,
            patientId,
            pharmacistId,
            startDate,
            endDate,
            statuses);

        return new CollectionDto<AppointmentMedicationsDto>(
            (int)HttpStatusCode.OK,
            "The appointment medications have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllAppointmentMedicationsList", "Retrieve all non-paginated appointment medications in the system.")]
    public ResponseDto<List<AppointmentMedicationsDto>> GetAllAppointmentMedications(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] Guid? appointmentId = null,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? pharmacistId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] DiagnosticStatus[]? statuses = null)
    {
        var result = appointmentMedicationsService.GetAllAppointmentMedications(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            appointmentId,
            doctorId,
            patientId,
            pharmacistId,
            startDate,
            endDate,
            statuses);

        return new ResponseDto<List<AppointmentMedicationsDto>>(
            (int)HttpStatusCode.OK,
            "The appointment medications have been successfully retrieved.",
            result);
    }

    [HttpGet("{appointmentMedicationsId:guid}")]
    [Documentation("GetAppointmentMedicationsById", "Retrieve the respective appointment medications via its identifier in the system.")]
    public ResponseDto<AppointmentMedicationsDto> GetAppointmentMedicationsById([FromRoute] Guid appointmentMedicationsId)
    {
        var result = appointmentMedicationsService.GetAppointmentMedicationsById(appointmentMedicationsId);

        return new ResponseDto<AppointmentMedicationsDto>(
            (int)HttpStatusCode.OK,
            "Appointment medications successfully fetched.",
            result);
    }

    [HttpPatch("{appointmentMedicationsId:guid}/dispense")]
    [Documentation("DispenseAppointmentMedications", "Dispenses the medications for an appointment.")]
    public ResponseDto<bool> DispenseAppointmentMedications([FromRoute] Guid appointmentMedicationsId)
    {
        appointmentMedicationsService.DispenseAppointmentMedications(appointmentMedicationsId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Appointment medications successfully dispensed.",
            true);
    }
}
