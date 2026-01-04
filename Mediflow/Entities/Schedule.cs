using System.ComponentModel.DataAnnotations;

namespace mediflow.Entities;

public sealed class Schedule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    // A schedule is for a single day (simple + practical)
    public DateOnly WorkDate { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public int SlotMinutes { get; set; } = 15;

    [MaxLength(120)]
    public string? Location { get; set; }

    public bool IsPublished { get; set; } = true;

    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}