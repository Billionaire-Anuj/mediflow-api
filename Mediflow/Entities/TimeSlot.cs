using Mediflow.Entities.Enums;

namespace mediflow.Entities;

public sealed class TimeSlot
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ScheduleId { get; set; }
    public Schedule Schedule { get; set; } = null!;

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public int Capacity { get; set; } = 1;
    public int BookedCount { get; set; } = 0;

    public TimeSlotStatus Status { get; set; } = TimeSlotStatus.Open;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}