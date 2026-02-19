using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class Appointment(
    Guid doctorId,
    Guid patientId,
    DateTime bookedDate,
    Guid timeslotId,
    TimeOnly appointmentTime,
    DateTime? cancelledDate,
    AppointmentStatus status,
    string? notes,
    string? symptoms,
    decimal fee,
    bool isPaidViaGateway,
    bool isPaidViaOfflineMedium,
    string? cancellationReason
) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Doctor))]
    public Guid DoctorId { get; private set; } = doctorId;

    [ForeignKey(nameof(Patient))]
    public Guid PatientId { get; private set; } = patientId;

    public DateTime BookedDate { get; private set; } = bookedDate;

    public Guid TimeslotId { get; private set; } = timeslotId;

    public TimeOnly AppointmentTime { get; private set; } = appointmentTime;

    public DateTime? CancelledDate { get; private set; } = cancelledDate;

    public AppointmentStatus Status { get; private set; } = status;

    public string? Notes { get; private set; } = notes;

    public string? Symptoms { get; private set; } = symptoms;

    public decimal Fee { get; private set; } = fee;

    public bool IsPaidViaGateway { get; private set; } = isPaidViaGateway;

    public bool IsPaidViaOfflineMedium { get; private set; } = isPaidViaOfflineMedium;

    public string? CancellationReason { get; private set; } = cancellationReason;

    public virtual User Doctor { get; set; } = User.Default;

    public virtual User Patient { get; set; } = User.Default;

    public virtual Timeslot Timeslot { get; set; } = Timeslot.Default;

    public virtual MedicalRecord MedicalRecord { get; set; } = MedicalRecord.Default;

    public static Appointment Default => new(Guid.Empty, Guid.Empty, DateTime.MinValue, Guid.Empty, TimeOnly.MinValue, null, AppointmentStatus.Scheduled, null, null, 0m, false, false, null);

    public virtual ICollection<AppointmentDiagnostics> AppointmentDiagnostics { get; private set; } = new List<AppointmentDiagnostics>();
}
