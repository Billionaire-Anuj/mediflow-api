using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.DTOs.Doctors;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Doctors.Schedules;
using Mediflow.Application.DTOs.Doctors.Schedules.Timeslots;

namespace Mediflow.API.Controllers;

public class DoctorController(IDoctorService doctorService) : BaseController<DoctorController>
{
    [HttpGet("profile")]
    [Documentation("GetDoctorProfile", "Retrieve the logged in doctor's profile.")]
    public ResponseDto<DoctorProfileDto> GetDoctorProfile()
    {
        var result = doctorService.GetDoctorProfile();

        return new ResponseDto<DoctorProfileDto>(
            (int)HttpStatusCode.OK,
            "Doctor profile successfully fetched.",
            result);
    }

    [HttpGet("{doctorId:guid}")]
    [Documentation("GetDoctorProfileById", "Retrieve the respective doctor profile via its identifier in the system.")]
    public ResponseDto<DoctorProfileDto> GetDoctorProfileById([FromRoute] Guid doctorId)
    {
        var result = doctorService.GetDoctorProfileById(doctorId);

        return new ResponseDto<DoctorProfileDto>(
            (int)HttpStatusCode.OK,
            "Doctor profile successfully fetched.",
            result);
    }

    [HttpGet]
    [Documentation("GetAllDoctorProfiles", "Retrieve all paginated doctor profiles in the system.")]
    public CollectionDto<DoctorProfileDto> GetAllDoctorProfiles(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? name = null,
        [FromQuery] string? username = null,
        [FromQuery] string? emailAddress = null,
        [FromQuery] string? address = null,
        [FromQuery] string? phoneNumber = null,
        [FromQuery] List<Guid>? specializationIds = null)
    {
        var result = doctorService.GetAllDoctorProfiles(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            name,
            username,
            emailAddress,
            address,
            phoneNumber,
            specializationIds);

        return new CollectionDto<DoctorProfileDto>(
            (int)HttpStatusCode.OK,
            "The doctor profiles have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllDoctorProfilesList", "Retrieve all non-paginated doctor profiles in the system.")]
    public ResponseDto<List<DoctorProfileDto>> GetAllDoctorProfiles(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? name = null,
        [FromQuery] string? username = null,
        [FromQuery] string? emailAddress = null,
        [FromQuery] string? address = null,
        [FromQuery] string? phoneNumber = null,
        [FromQuery] List<Guid>? specializationIds = null)
    {
        var result = doctorService.GetAllDoctorProfiles(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            name,
            username,
            emailAddress,
            address,
            phoneNumber,
            specializationIds);

        return new ResponseDto<List<DoctorProfileDto>>(
            (int)HttpStatusCode.OK,
            "The doctor profiles have been successfully retrieved.",
            result);
    }

    [HttpPut("profile")]
    [Documentation("UpdateDoctorProfile", "Updates the logged in doctor's profile.")]
    public ResponseDto<bool> UpdateDoctorProfile([FromBody] UpdateDoctorProfileDto doctorProfile)
    {
        doctorService.UpdateDoctorProfile(doctorProfile);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Doctor profile successfully updated.",
            true);
    }

    [HttpPost("schedules")]
    [Documentation("CreateDoctorSchedule", "Creates a schedule for the logged in doctor.")]
    public ResponseDto<bool> CreateDoctorSchedule([FromBody] CreateScheduleDto doctorSchedule)
    {
        doctorService.CreateDoctorSchedule(doctorSchedule);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Doctor schedule successfully created.",
            true);
    }

    [HttpPut("schedules/{scheduleId:guid}")]
    [Documentation("UpdateDoctorSchedule", "Updates a doctor's schedule.")]
    public ResponseDto<bool> UpdateDoctorSchedule([FromRoute] Guid scheduleId, [FromBody] UpdateScheduleDto doctorSchedule)
    {
        doctorService.UpdateDoctorSchedule(scheduleId, doctorSchedule);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Doctor schedule successfully updated.",
            true);
    }

    [HttpGet("timeslots")]
    [Documentation("GetDoctorTimeslots", "Retrieve timeslots of the profile in a date range.")]
    public ResponseDto<List<TimeslotDto>> GetDoctorTimeslots(
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null)
    {
        var result = doctorService.GetDoctorTimeslots(startDate, endDate);

        return new ResponseDto<List<TimeslotDto>>(
            (int)HttpStatusCode.OK,
            "Doctor timeslots successfully fetched.",
            result);
    }

    [HttpGet("{doctorId:guid}/timeslots")]
    [Documentation("GetDoctorTimeslotsById", "Retrieve timeslots for a doctor in a date range.")]
    public ResponseDto<List<TimeslotDto>> GetDoctorTimeslotsById(
        [FromRoute] Guid doctorId,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null)
    {
        var result = doctorService.GetDoctorTimeslotsById(doctorId, startDate, endDate);

        return new ResponseDto<List<TimeslotDto>>(
            (int)HttpStatusCode.OK,
            "Doctor timeslots successfully fetched.",
            result);
    }
}
