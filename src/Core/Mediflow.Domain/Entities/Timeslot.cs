using System.ComponentModel.DataAnnotations.Schema;
using Mediflow.Domain.Common.Base;

namespace Mediflow.Domain.Entities;

public class Timeslot(Guid scheduleId, DateOnly date, TimeOnly startTime, TimeOnly endTime, bool isBooked = false) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Schedule))]
    public Guid ScheduleId { get; private set; } = scheduleId;

    public DateOnly Date { get; private set; } = date;

    public TimeOnly StartTime { get; private set; } = startTime;

    public TimeOnly EndTime { get; private set; } = endTime;

    public bool IsBooked { get; private set; } = isBooked;

    public static Timeslot Default => new(Guid.Empty, DateOnly.MinValue, TimeOnly.MinValue, TimeOnly.MinValue);

    public virtual Schedule? Schedule { get; set; }

    public virtual Appointment? Appointment { get; set; }
}