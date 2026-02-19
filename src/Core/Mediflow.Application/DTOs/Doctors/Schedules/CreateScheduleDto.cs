namespace Mediflow.Application.DTOs.Doctors.Schedules;

public class CreateScheduleDto
{
    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int SlotDurationInMinutes { get; set; }

    public DateOnly ValidStartDate { get; set; }

    public DateOnly ValidEndDate { get; set; }

    public string Notes { get; set; } = string.Empty;
}