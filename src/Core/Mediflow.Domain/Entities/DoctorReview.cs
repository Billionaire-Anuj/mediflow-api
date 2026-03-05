using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class DoctorReview(
    Guid appointmentId,
    Guid doctorId,
    Guid patientId,
    int rating,
    string? review) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Appointment))]
    public Guid AppointmentId { get; private set; } = appointmentId;

    [ForeignKey(nameof(Doctor))]
    public Guid DoctorId { get; private set; } = doctorId;

    [ForeignKey(nameof(Patient))]
    public Guid PatientId { get; private set; } = patientId;

    public int Rating { get; private set; } = rating;

    public string? Review { get; private set; } = review;

    public virtual Appointment? Appointment { get; set; }

    public virtual User? Doctor { get; set; }

    public virtual User? Patient { get; set; }

    public static DoctorReview Default => new(Guid.Empty, Guid.Empty, Guid.Empty, 0, null);
}
