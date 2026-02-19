using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class Schedule(
    Guid doctorId,
    DayOfWeek dayOfWeek,
    TimeOnly startTime,
    TimeOnly endTime,
    int slotDurationInMinutes = 30,
    bool isAvailable = true,
    DateOnly validStartDate = default,
    DateOnly validEndDate = default,
    string? notes = null
) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Doctor))]
    public Guid DoctorId { get; private set; } = doctorId;

    public DayOfWeek DayOfWeek { get; private set; } = dayOfWeek;

    public TimeOnly StartTime { get; private set; } = startTime;

    public TimeOnly EndTime { get; private set; } = endTime;

    public int SlotDurationInMinutes { get; private set; } = slotDurationInMinutes;

    public bool IsAvailable { get; private set; } = isAvailable;

    public DateOnly ValidStartDate { get; private set; } = validStartDate;

    public DateOnly ValidEndDate { get; private set; } = validEndDate;

    public string Notes { get; private set; } = notes ?? string.Empty;

    public static Schedule Default => new(Guid.Empty, DayOfWeek.Monday, TimeOnly.MinValue, TimeOnly.MinValue);

    public virtual User? Doctor { get; set; }

    public virtual List<Timeslot> Timeslots { get; set; } = new();

    public void Update(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int slotDurationInMinutes, DateOnly validStartDate, DateOnly validEndDate, string notes)
    {
        if (DayOfWeek != dayOfWeek) DayOfWeek = dayOfWeek;
        if (StartTime != startTime) StartTime = startTime;
        if (EndTime != endTime) EndTime = endTime;
        if (SlotDurationInMinutes != slotDurationInMinutes) SlotDurationInMinutes = slotDurationInMinutes;
        if (ValidStartDate != validStartDate) ValidStartDate = validStartDate;
        if (ValidEndDate != validEndDate) ValidEndDate = validEndDate;
        if (Notes != notes) Notes = notes;
    }

}