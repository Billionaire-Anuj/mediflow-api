using mediflow.Entities;
using Mediflow.Entities.Enums;

namespace mediflow.Services;

public interface ITimeSlotGenerator
{
    List<TimeSlot> Generate(Guid scheduleId, TimeOnly start, TimeOnly end, int slotMinutes);
}

public sealed class TimeSlotGenerator : ITimeSlotGenerator
{
    public List<TimeSlot> Generate(Guid scheduleId, TimeOnly start, TimeOnly end, int slotMinutes)
    {
        if (slotMinutes <= 0) throw new ArgumentOutOfRangeException(nameof(slotMinutes));
        if (end <= start) throw new ArgumentException("EndTime must be after StartTime.");

        var slots = new List<TimeSlot>();
        var cursor = start;

        while (cursor.AddMinutes(slotMinutes) <= end)
        {
            var slotEnd = cursor.AddMinutes(slotMinutes);

            slots.Add(new TimeSlot
            {
                ScheduleId = scheduleId,
                StartTime = cursor,
                EndTime = slotEnd,
                Capacity = 1,
                BookedCount = 0,
                Status = TimeSlotStatus.Open
            });

            cursor = slotEnd;
        }

        return slots;
    }
}