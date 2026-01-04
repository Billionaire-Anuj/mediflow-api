namespace mediflow.Contracts;

public sealed record CreateScheduleRequest(
    DateOnly WorkDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int? SlotMinutes,
    string? Location,
    bool? Publish
);

public sealed record ScheduleDto(
    Guid Id,
    Guid DoctorId,
    DateOnly WorkDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int SlotMinutes,
    string? Location,
    bool IsPublished
);

public sealed record TimeSlotDto(
    Guid Id,
    Guid ScheduleId,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int Capacity,
    int BookedCount,
    string Status
);