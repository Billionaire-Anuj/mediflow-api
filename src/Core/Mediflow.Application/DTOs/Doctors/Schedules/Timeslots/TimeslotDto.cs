using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Doctors.Schedules.Timeslots;

public class TimeslotDto : BaseDto
{
    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsBooked { get; set; }
}