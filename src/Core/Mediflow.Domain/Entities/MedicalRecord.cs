using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class MedicalRecord(
    Guid appointmentId,
    string diagnosis,
    string treatment,
    string prescriptions,
    string? notes = null
) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Appointment))]
    public Guid AppointmentId { get; private set; } = appointmentId;

    public string Diagnosis { get; private set; } = diagnosis;

    public string Treatment { get; private set; } = treatment;

    public string Prescriptions { get; private set; } = prescriptions;

    public string Notes { get; private set; } = notes ?? string.Empty;

    public virtual Appointment? Appointment { get; set; }

    public static MedicalRecord Default => new(Guid.Empty, string.Empty, string.Empty, string.Empty);

    public void Update(string diagnosis, string treatment, string prescriptions, string? notes)
    {
        if (Diagnosis != diagnosis) Diagnosis = diagnosis;
        if (Treatment != treatment) Treatment = treatment;
        if (Prescriptions != prescriptions) Prescriptions = prescriptions;
        if (Notes != notes) Notes = notes ?? string.Empty;
    }
}