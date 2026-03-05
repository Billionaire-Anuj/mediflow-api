using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.Domain.Common.Enum;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Appointments;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Reviews;

namespace Mediflow.API.Controllers;

public class AppointmentController(
    IAppointmentService appointmentService,
    IDoctorReviewService doctorReviewService) : BaseController<AppointmentController>
{
    [HttpGet]
    [Documentation("GetAllAppointments", "Retrieve all paginated appointments in the system.")]
    public CollectionDto<AppointmentDto> GetAllAppointments(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] AppointmentStatus[]? statuses = null)
    {
        var result = appointmentService.GetAllAppointments(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            doctorId,
            patientId,
            startDate,
            endDate,
            statuses);

        return new CollectionDto<AppointmentDto>(
            (int)HttpStatusCode.OK,
            "The appointments have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllAppointmentsList", "Retrieve all non-paginated appointments in the system.")]
    public ResponseDto<List<AppointmentDto>> GetAllAppointments(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] AppointmentStatus[]? statuses = null)
    {
        var result = appointmentService.GetAllAppointments(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            doctorId,
            patientId,
            startDate,
            endDate,
            statuses);

        return new ResponseDto<List<AppointmentDto>>(
            (int)HttpStatusCode.OK,
            "The appointments have been successfully retrieved.",
            result);
    }

    [HttpGet("{appointmentId:guid}")]
    [Documentation("GetAppointmentById", "Retrieve the respective appointment via its identifier in the system.")]
    public ResponseDto<AppointmentDto> GetAppointmentById([FromRoute] Guid appointmentId)
    {
        var result = appointmentService.GetAppointmentById(appointmentId);

        return new ResponseDto<AppointmentDto>(
            (int)HttpStatusCode.OK,
            "Appointment successfully fetched.",
            result);
    }

    [HttpPost]
    [Documentation("BookAppointment", "Books a new appointment for a patient.")]
    public ResponseDto<bool> BookAppointment([FromBody] CreateAppointmentDto appointment)
    {
        appointmentService.BookAppointment(appointment);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Appointment successfully booked.",
            true);
    }

    [HttpPut("{appointmentId:guid}")]
    [Documentation("UpdateAppointment", "Updates an existing appointment details.")]
    public ResponseDto<bool> UpdateAppointment([FromRoute] Guid appointmentId, [FromBody] UpdateAppointmentDto appointment)
    {
        appointmentService.UpdateAppointment(appointmentId, appointment);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Appointment successfully updated.",
            true);
    }

    [HttpPatch("{appointmentId:guid}/cancel")]
    [Documentation("CancelAppointment", "Cancels a scheduled appointment.")]
    public ResponseDto<bool> CancelAppointment([FromRoute] Guid appointmentId, [FromBody] CancelAppointmentDto appointment)
    {
        appointmentService.CancelAppointment(appointmentId, appointment);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Appointment successfully cancelled.",
            true);
    }

    [HttpPatch("{appointmentId:guid}/consult")]
    [Documentation("ConsultAppointment", "Consults the appointment and records diagnostics and medications.")]
    public ResponseDto<bool> ConsultAppointment([FromRoute] Guid appointmentId, [FromBody] ConsultAppointmentDto appointment)
    {
        appointmentService.ConsultAppointment(appointmentId, appointment);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Appointment successfully consulted.",
            true);
    }

    [HttpPost("{appointmentId:guid}/review")]
    [Documentation("CreateDoctorReview", "Creates a review for a completed appointment.")]
    public ResponseDto<bool> CreateDoctorReview([FromRoute] Guid appointmentId, [FromBody] CreateDoctorReviewDto review)
    {
        doctorReviewService.CreateDoctorReview(appointmentId, review);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Review successfully submitted.",
            true);
    }

    [HttpPatch("{appointmentId:guid}/pay/credits")]
    [Documentation("PayAppointmentWithCredits", "Pays for an appointment using patient credits.")]
    public ResponseDto<bool> PayAppointmentWithCredits([FromRoute] Guid appointmentId)
    {
        appointmentService.PayAppointmentWithCredits(appointmentId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Appointment paid successfully using credits.",
            true);
    }
}
