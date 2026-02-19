using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Doctors.Schedules;

public static class ScheduleExtensionMethods
{
    public static ScheduleDto ToScheduleDto(this Schedule schedule)
    {
        return new ScheduleDto
        {
            Id = schedule.Id,
            Notes = schedule.Notes,
            EndTime = schedule.EndTime,
            IsActive = schedule.IsActive,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            IsAvailable = schedule.IsAvailable,
            ValidEndDate = schedule.ValidEndDate,
            ValidStartDate = schedule.ValidStartDate,
            SlotDurationInMinutes = schedule.SlotDurationInMinutes,
        };
    }
}