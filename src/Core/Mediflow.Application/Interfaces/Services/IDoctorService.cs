using Mediflow.Application.DTOs.Doctors;
using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Doctors.Schedules;
using Mediflow.Application.DTOs.Doctors.Schedules.Timeslots;

namespace Mediflow.Application.Interfaces.Services;

public interface IDoctorService : ITransientService
{
    #region Profile
    DoctorProfileDto GetDoctorProfile();

    DoctorProfileDto GetDoctorProfile(Guid doctorId);

    List<DoctorProfileDto> GetAllDoctorProfiles(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? username = null,
        string? emailAddress = null,
        string? address = null,
        string? phoneNumber = null,
        List<Guid>? specializationIds = null);

    List<DoctorProfileDto> GetAllDoctorProfiles(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? username = null,
        string? emailAddress = null,
        string? address = null,
        string? phoneNumber = null,
        List<Guid>? specializationIds = null);

    void UpdateDoctorProfile(UpdateDoctorProfileDto doctorProfile);
    #endregion

    #region Schedules & Timeslots
    void CreateDoctorSchedule(CreateScheduleDto doctorSchedule);

    void UpdateDoctorSchedule(Guid scheduleId, UpdateScheduleDto doctorSchedule);

    List<TimeslotDto> GetDoctorTimeslots(Guid doctorId, DateOnly? startDate, DateOnly? endDate);
    #endregion
}