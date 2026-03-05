using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class Appointment(
    Guid doctorId,
    Guid patientId,
    DateTime bookedDate,
    Guid timeslotId,
    DateTime? cancelledDate,
    AppointmentStatus status,
    string? notes,
    string? symptoms,
    decimal fee,
    bool isPaidViaGateway = false,
    bool isPaidViaOfflineMedium = false,
    string? cancellationReason = null
) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Doctor))]
    public Guid DoctorId { get; private set; } = doctorId;

    [ForeignKey(nameof(Patient))]
    public Guid PatientId { get; private set; } = patientId;

    public DateTime BookedDate { get; private set; } = bookedDate;

    public Guid TimeslotId { get; private set; } = timeslotId;

    public DateTime? CancelledDate { get; private set; } = cancelledDate;

    public AppointmentStatus Status { get; private set; } = status;

    public string? Notes { get; private set; } = notes;

    public string? Symptoms { get; private set; } = symptoms;

    public decimal Fee { get; private set; } = fee;

    public bool IsPaidViaGateway { get; private set; } = isPaidViaGateway;

    public bool IsPaidViaOfflineMedium { get; private set; } = isPaidViaOfflineMedium;

    public string? CancellationReason { get; private set; } = cancellationReason;

    public virtual User? Doctor { get; set; }

    public virtual User? Patient { get; set; }

    public virtual Timeslot? Timeslot { get; set; }

    public virtual MedicalRecord? MedicalRecord { get; set; }

    public virtual DoctorReview? Review { get; set; }

    public static Appointment Default => new(Guid.Empty, Guid.Empty, DateTime.MinValue, Guid.Empty, null, AppointmentStatus.Scheduled, null, null, 0m, false, false, null);

    public virtual ICollection<AppointmentDiagnostics> AppointmentDiagnostics { get; private set; } = new List<AppointmentDiagnostics>();

    public virtual ICollection<AppointmentMedications> AppointmentMedications { get; private set; } = new List<AppointmentMedications>();

    public void UpdateDetails(Guid timeslotId, string? notes, string? symptoms)
    {
        if (TimeslotId != timeslotId) TimeslotId = timeslotId;
        if (Notes != notes) Notes = notes;
        if (Symptoms != symptoms) Symptoms = symptoms;
    }

    public void MarkCompleted()
    {
        if (Status != AppointmentStatus.Completed)
        {
            Status = AppointmentStatus.Completed;
        }
    }

    public void Cancel(string cancellationReason, DateTime cancelledDate)
    {
        if (Status != AppointmentStatus.Canceled)
        {
            Status = AppointmentStatus.Canceled;
        }

        if (CancelledDate != cancelledDate) CancelledDate = cancelledDate;
        if (CancellationReason != cancellationReason) CancellationReason = cancellationReason;
    }

    public void MarkPaidViaGateway()
    {
        if (IsPaidViaGateway)
        {
            return;
        }

        IsPaidViaGateway = true;
    }
}
