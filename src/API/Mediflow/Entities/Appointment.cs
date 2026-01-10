using System.ComponentModel.DataAnnotations;
using Mediflow.Entities.Enums;

namespace mediflow.Entities;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid TimeSlotId { get; set; }
    public TimeSlot TimeSlot { get; set; } = null!;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;

    [MaxLength(200)]
    public string? Reason { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    public int PointsUsed { get; set; } = 0;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}