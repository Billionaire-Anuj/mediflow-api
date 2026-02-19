using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Doctors.Schedules.Timeslots;

public static class TimeslotExtensionMethods
{
    public static TimeslotDto ToTimeslotDto(this Timeslot timeslot)
    {
        return new TimeslotDto()
        {
            Id = timeslot.Id,
            Date = timeslot.Date,
            EndTime = timeslot.EndTime,
            IsBooked = timeslot.IsBooked,
            IsActive = timeslot.IsActive,
            StartTime = timeslot.StartTime
        };
    }
}