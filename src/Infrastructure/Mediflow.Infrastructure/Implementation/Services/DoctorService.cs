using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.DTOs.Doctors;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Doctors.Schedules;
using Mediflow.Application.DTOs.Doctors.Schedules.Timeslots;

namespace Mediflow.Infrastructure.Implementation.Services;

public class DoctorService(
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService) : IDoctorService
{
    public DoctorProfileDto GetDoctorProfile()
    {
        var userId = applicationUserService.GetUserId;

        var doctor = applicationDbContext.Users
                       .AsNoTracking()
                       .Include(x => x.Role)
                       .Include(x => x.DoctorProfile)
                       .Include(x => x.Schedules)
                       .Include(x => x.DoctorSpecializations)
                          .ThenInclude(x => x.Specialization)
                       .FirstOrDefault(x => x.Id == userId)
                   ?? throw new NotFoundException($"Doctor with the identifier of {userId} could not be found.");

        return doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id 
            ? throw new BadRequestException("Only users with the doctor role can update their doctor profile.")
            : doctor.ToDoctorProfileDto();
    }

    public DoctorProfileDto GetDoctorProfileById(Guid doctorId)
    {
        var doctor = applicationDbContext.Users
                         .AsNoTracking()
                         .Include(x => x.Role)
                         .Include(x => x.DoctorProfile)
                         .Include(x => x.Schedules)
                         .Include(x => x.DoctorSpecializations)
                         .ThenInclude(x => x.Specialization)
                         .FirstOrDefault(x => x.Id == doctorId)
                     ?? throw new NotFoundException($"Doctor with the identifier of {doctorId} could not be found.");

        return doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id 
            ? throw new BadRequestException("Only users with the doctor role can update their doctor profile.")
            : doctor.ToDoctorProfileDto();
    }

    public List<DoctorProfileDto> GetAllDoctorProfiles(
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
        List<Guid>? specializationIds = null)
    {
        var role = applicationDbContext.Roles
            .AsNoTracking()
            .FirstOrDefault(x => x.Id.ToString() == Constants.Roles.Doctor.Id)
            ?? throw new NotFoundException($"Doctor role with the identifier of {Constants.Roles.Doctor.Id} could not be found.");

        var specializationIdentifiers = specializationIds != null ? new HashSet<Guid>(specializationIds) : null;

        var doctorModels = applicationDbContext.Users
            .Where(x =>
                x.RoleId == role.Id &&
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Name.ToLower().Contains(globalSearch.ToLower())
                    || x.Username.ToLower().Contains(globalSearch.ToLower())
                    || x.EmailAddress.ToLower().Contains(globalSearch.ToLower())
                    || (x.Address != null && x.Address.ToLower().Contains(globalSearch.ToLower()))
                    || x.PhoneNumber.ToLower().Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (name == null || x.Name.ToLower().Contains(name.ToLower())) &&
                (username == null || x.Username.ToLower().Contains(username.ToLower())) &&
                (emailAddress == null || x.EmailAddress.ToLower().Contains(emailAddress.ToLower())) &&
                (address == null || (x.Address != null && x.Address.ToLower().Contains(address.ToLower()))) &&
                (phoneNumber == null || x.PhoneNumber.ToLower().Contains(phoneNumber.ToLower())) &&
                (specializationIdentifiers == null || x.DoctorSpecializations.Any(z => specializationIdentifiers.Contains(z.SpecializationId))))
            .Include(x => x.Role)
            .Include(x => x.DoctorProfile)
            .Include(x => x.Schedules)
            .Include(x => x.DoctorSpecializations)
                .ThenInclude(x => x.Specialization)
            .OrderBy(x => orderBys);

        rowCount = doctorModels.Count();

        return doctorModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToDoctorProfileDto())
            .ToList();
    }

    public List<DoctorProfileDto> GetAllDoctorProfiles(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? username = null,
        string? emailAddress = null,
        string? address = null,
        string? phoneNumber = null,
        List<Guid>? specializationIds = null)
    {
        var role = applicationDbContext.Roles
                       .AsNoTracking()
                       .FirstOrDefault(x => x.Id.ToString() == Constants.Roles.Doctor.Id)
                   ?? throw new NotFoundException($"Doctor role with the identifier of {Constants.Roles.Doctor.Id} could not be found.");

        var specializationIdentifiers = specializationIds != null ? new HashSet<Guid>(specializationIds) : null;

        var doctorModels = applicationDbContext.Users
            .Where(x =>
                x.RoleId == role.Id &&
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Name.ToLower().Contains(globalSearch.ToLower())
                    || x.Username.ToLower().Contains(globalSearch.ToLower())
                    || x.EmailAddress.ToLower().Contains(globalSearch.ToLower())
                    || (x.Address != null && x.Address.ToLower().Contains(globalSearch.ToLower()))
                    || x.PhoneNumber.ToLower().Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (name == null || x.Name.ToLower().Contains(name.ToLower())) &&
                (username == null || x.Username.ToLower().Contains(username.ToLower())) &&
                (emailAddress == null || x.EmailAddress.ToLower().Contains(emailAddress.ToLower())) &&
                (address == null || (x.Address != null && x.Address.ToLower().Contains(address.ToLower()))) &&
                (phoneNumber == null || x.PhoneNumber.ToLower().Contains(phoneNumber.ToLower())) &&
                (specializationIdentifiers == null || x.DoctorSpecializations.Any(z => specializationIdentifiers.Contains(z.SpecializationId))))
            .Include(x => x.Role)
            .Include(x => x.DoctorProfile)
            .Include(x => x.Schedules)
            .Include(x => x.DoctorSpecializations)
            .ThenInclude(x => x.Specialization)
            .OrderBy(x => orderBys);

        return doctorModels.Select(x => x.ToDoctorProfileDto()).ToList();
    }

    public void UpdateDoctorProfile(UpdateDoctorProfileDto doctorProfile)
    {
        var userId = applicationUserService.GetUserId;

        var doctor = applicationDbContext.Users
                         .Include(x => x.Role)
                         .Include(x => x.DoctorProfile)
                         .AsNoTracking()
                         .FirstOrDefault(x => x.Id == userId) 
                     ?? throw new NotFoundException($"User with the identifier of {userId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException("Only users with the doctor role can update their doctor profile.");

        var doctorProfileModel = doctor.DoctorProfile;

        if (doctorProfileModel == null)
        {
            doctorProfileModel = new DoctorProfile(doctor.Id, doctorProfile.About, doctorProfile.LicenseNumber, doctorProfile.EducationInformation, doctorProfile.ExperienceInformation, doctorProfile.ConsultationFee);

            applicationDbContext.DoctorProfiles.Add(doctorProfileModel);
        }
        else
        {
            doctorProfileModel.Update(doctorProfile.About, doctorProfile.LicenseNumber, doctorProfile.EducationInformation, doctorProfile.ExperienceInformation, doctorProfile.ConsultationFee);
        }

        var doctorSpecializations = applicationDbContext.DoctorSpecializations.Where(x => x.DoctorId == doctor.Id).ToList();

        if (doctorSpecializations.Count != 0)
        {
            applicationDbContext.DoctorSpecializations.RemoveRange(doctorSpecializations);            
        }

        var specializationModels = applicationDbContext.Specializations
            .AsNoTracking()
            .Where(x => doctorProfile.SpecializationIds.Contains(x.Id))
            .ToList();

        var specializationDictionary = specializationModels.ToDictionary(x => x.Id);

        var doctorSpecializationModels = doctorProfile.SpecializationIds
            .Select(
                x => specializationDictionary.TryGetValue(x, out var specializationModel)
                    ? new DoctorSpecialization(doctor.Id, specializationModel.Id)
                    : throw new NotFoundException($"Specialization with the identifier of {x} could not be found.")
            ).ToList();

        applicationDbContext.DoctorSpecializations.AddRange(doctorSpecializationModels);

        applicationDbContext.SaveChanges();
    }

    public void CreateDoctorSchedule(CreateScheduleDto doctorSchedule)
    {
        var userId = applicationUserService.GetUserId;

        var doctor = applicationDbContext.Users
                         .Include(x => x.Role)
                         .Include(x => x.DoctorProfile)
                         .AsNoTracking()
                         .FirstOrDefault(x => x.Id == userId) 
                     ?? throw new NotFoundException($"User with the identifier of {userId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException("Only users with the doctor role can update their doctor profile.");

        if (doctorSchedule.StartTime >= doctorSchedule.EndTime)
            throw new BadRequestException("Start time must be earlier than the end time.");

        if (doctorSchedule.SlotDurationInMinutes <= 0)
            throw new BadRequestException("Slot duration in minutes must be greater than 0.");

        var startDate = doctorSchedule.ValidStartDate == default
            ? DateOnly.FromDateTime(DateTime.Now.Date)
            : doctorSchedule.ValidStartDate;

        var endDate = doctorSchedule.ValidEndDate == default
            ? startDate.AddMonths(3)
            : doctorSchedule.ValidEndDate;

        if (endDate < startDate)
            throw new BadRequestException("Valid end date must be on or after the valid start date.");

        var day = doctorSchedule.DayOfWeek;
        var startTime = doctorSchedule.StartTime;
        var endTime = doctorSchedule.EndTime;

        var duplicateSchedule = applicationDbContext.Schedules
            .AsNoTracking()
            .Any(x => 
                x.DoctorId == doctor.Id &&
                x.DayOfWeek == day &&
                x.StartTime < endTime &&
                x.EndTime > startTime &&
                x.ValidStartDate <= endDate &&
                x.ValidEndDate >= startDate
            );

        if (duplicateSchedule)
            throw new BadRequestException("A schedule already exists that overlaps with the given day or time and validity period.");

        var schedule = new Schedule(
            doctor.Id,
            day,
            startTime,
            endTime,
            doctorSchedule.SlotDurationInMinutes,
            true,
            startDate,
            endDate,
            doctorSchedule.Notes
        );

        applicationDbContext.Schedules.Add(schedule);
        applicationDbContext.SaveChanges();

        if (schedule.IsAvailable)
        {
            var timeSlots = applicationDbContext.Timeslots.AsNoTracking()
                .Where(x => x.ScheduleId == schedule.Id && x.Date >= schedule.ValidStartDate && x.Date <= schedule.ValidEndDate)
                .Select(x => new
                {
                    x.Date,
                    x.StartTime,
                    x.EndTime
                })
                .ToList();

            var timeSlotSet = new HashSet<string>(timeSlots.Select(x => $"{x.Date:yyyy-MM-dd}|{x.StartTime:HH:mm}|{x.EndTime:HH:mm}"));

            foreach (var date in EachDate(schedule.ValidStartDate, schedule.ValidEndDate))
            {
                if (date.DayOfWeek != schedule.DayOfWeek) continue;

                var cursor = startTime;

                while (cursor.AddMinutes(schedule.SlotDurationInMinutes) <= endTime)
                {
                    var slotStart = cursor;
                    var slotEnd = cursor.AddMinutes(schedule.SlotDurationInMinutes);

                    var key = $"{date:yyyy-MM-dd}|{slotStart:HH:mm}|{slotEnd:HH:mm}";

                    if (!timeSlotSet.Contains(key))
                    {
                        var timeslotModel = new Timeslot(schedule.Id, date, slotStart, slotEnd);

                        applicationDbContext.Timeslots.Add(timeslotModel);

                        timeSlotSet.Add(key);
                    }

                    cursor = slotEnd;
                }
            }
        }

        applicationDbContext.SaveChanges();
    }

    public void UpdateDoctorSchedule(Guid scheduleId, UpdateScheduleDto doctorSchedule)
    {
        if (scheduleId != doctorSchedule.Id)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var userId = applicationUserService.GetUserId;

        var doctor = applicationDbContext.Users
            .Include(x => x.Role)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"User with the identifier of {userId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException("Only users with the doctor role can update their schedules.");

        var schedule = applicationDbContext.Schedules
                           .FirstOrDefault(x => x.Id == doctorSchedule.Id) 
                       ?? throw new NotFoundException($"Schedule with the identifier of {doctorSchedule.Id} could not be found.");

        if (schedule.DoctorId != doctor.Id)
            throw new BadRequestException("You can only update your own schedules.");

        if (doctorSchedule.StartTime >= doctorSchedule.EndTime)
            throw new BadRequestException("Start time must be earlier than the end time.");

        if (doctorSchedule.SlotDurationInMinutes <= 0)
            throw new BadRequestException("Slot duration in minutes must be greater than 0.");

        var startDate = doctorSchedule.ValidStartDate == default
            ? DateOnly.FromDateTime(DateTime.Now.Date)
            : doctorSchedule.ValidStartDate;

        var endDate = doctorSchedule.ValidEndDate == default
            ? startDate.AddMonths(3)
            : doctorSchedule.ValidEndDate;

        if (endDate < startDate)
            throw new BadRequestException("Valid end date must be on or after the valid start date.");

        var day = doctorSchedule.DayOfWeek;
        var startTime = doctorSchedule.StartTime;
        var endTime = doctorSchedule.EndTime;

        var duplicateSchedule = applicationDbContext.Schedules
            .AsNoTracking()
            .Any(x =>
                x.Id != schedule.Id &&
                x.DoctorId == doctor.Id &&
                x.DayOfWeek == day &&
                x.StartTime < endTime &&
                x.EndTime > startTime &&
                x.ValidStartDate <= endDate &&
                x.ValidEndDate >= startDate
            );

        if (duplicateSchedule)
            throw new BadRequestException("Another schedule overlaps with the given day/time and validity period.");

        var hasAppointments = applicationDbContext.Appointments
            .AsNoTracking()
            .Any(x => x.Timeslot != null && x.Timeslot.ScheduleId == schedule.Id);

        if (hasAppointments)
            throw new BadRequestException("Cannot update this schedule because appointments already exist for its timeslots.");

        var timeslots = applicationDbContext.Timeslots
            .Where(x => x.ScheduleId == schedule.Id)
            .ToList();

        if (timeslots.Count != 0)
            applicationDbContext.Timeslots.RemoveRange(timeslots);

        schedule.Update(
            day,
            startTime,
            endTime,
            doctorSchedule.SlotDurationInMinutes,
            startDate,
            endDate,
            doctorSchedule.Notes
        );

        if (schedule.IsAvailable)
        {
            var slotModels = new List<Timeslot>();

            foreach (var date in EachDate(startDate, endDate))
            {
                if (date.DayOfWeek != day) continue;

                var cursor = startTime;

                while (cursor.AddMinutes(schedule.SlotDurationInMinutes) <= endTime)
                {
                    var slotStart = cursor;
                    var slotEnd = cursor.AddMinutes(schedule.SlotDurationInMinutes);

                    slotModels.Add(new Timeslot(schedule.Id, date, slotStart, slotEnd, isBooked: false));

                    cursor = slotEnd;
                }
            }

            if (slotModels.Count != 0) applicationDbContext.Timeslots.AddRange(slotModels);
        }

        applicationDbContext.SaveChanges();
    }

    public List<TimeslotDto> GetDoctorTimeslots(DateOnly? startDate, DateOnly? endDate)
    {
        var userId = applicationUserService.GetUserId;

        var doctor = applicationDbContext.Users
                         .AsNoTracking()
                         .Include(x => x.Role)
                         .Include(x => x.DoctorProfile)
                         .Include(x => x.Schedules)
                         .Include(x => x.DoctorSpecializations)
                         .ThenInclude(x => x.Specialization)
                         .FirstOrDefault(x => x.Id == userId)
                     ?? throw new NotFoundException($"Doctor with the identifier of {userId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException($"Doctor with the identifier of {userId} could not be found.");

        var timeslotModels = applicationDbContext.Timeslots
            .Include(x => x.Schedule)
            .Where(x =>
                x.Schedule!.DoctorId == doctor.Id &&
                (!startDate.HasValue || x.Date >= startDate.Value) &&
                (!endDate.HasValue || x.Date <= endDate.Value))
            .OrderBy(x => x.StartTime)
            .AsNoTracking();

        return timeslotModels.Select(x => x.ToTimeslotDto()).ToList();
    }

    public List<TimeslotDto> GetDoctorTimeslotsById(Guid doctorId, DateOnly? startDate, DateOnly? endDate)
    {
        var doctor = applicationDbContext.Users
                         .AsNoTracking()
                         .Include(x => x.Role)
                         .Include(x => x.DoctorProfile)
                         .Include(x => x.Schedules)
                         .Include(x => x.DoctorSpecializations)
                         .ThenInclude(x => x.Specialization)
                         .FirstOrDefault(x => x.Id == doctorId)
                     ?? throw new NotFoundException($"Doctor with the identifier of {doctorId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException($"Doctor with the identifier of {doctorId} could not be found.");

        var timeslotModels = applicationDbContext.Timeslots
            .Include(x => x.Schedule)
            .Where(x =>
                x.Schedule!.DoctorId == doctor.Id &&
                (!startDate.HasValue || x.Date >= startDate.Value) &&
                (!endDate.HasValue || x.Date <= endDate.Value))
            .OrderBy(x => x.StartTime)
            .AsNoTracking();

        return timeslotModels.Select(x => x.ToTimeslotDto()).ToList();
    }

    #region Private Methods
    private static IEnumerable<DateOnly> EachDate(DateOnly startDate, DateOnly endDate)
    {
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
            yield return date;
    }
    #endregion
}