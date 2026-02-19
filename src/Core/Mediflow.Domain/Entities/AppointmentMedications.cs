using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class AppointmentMedications(
    Guid appointmentId,
    Guid? pharmacistId,
    string notes,
    DiagnosticStatus status = DiagnosticStatus.Appointed) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Appointment))]
    public Guid AppointmentId { get; private set; } = appointmentId;

    [ForeignKey(nameof(Pharmacist))]
    public Guid? PharmacistId { get; private set; } = pharmacistId;

    public string Notes { get; private set; } = notes;

    public DiagnosticStatus Status { get; private set; } = status;

    public DateTime? CompletedDate { get; private set; }

    public static AppointmentMedications Default => new(Guid.Empty, null, string.Empty);

    public virtual Appointment? Appointment { get; set; }

    public virtual User? Pharmacist { get; set; }

    public virtual ICollection<AppointmentMedicationDrugs> Drugs { get; set; } = new List<AppointmentMedicationDrugs>();

    public void AssignPharmacist(Guid? pharmacistId)
    {
        if (PharmacistId != pharmacistId) PharmacistId = pharmacistId;
    }

    public void UpdateNotes(string notes)
    {
        if (Notes != notes) Notes = notes;
    }

    public void UpdateStatus(DiagnosticStatus status)
    {
        if (Status != status) Status = status;
    }

    public void MarkCollected(Guid pharmacistId, DateTime completedDate)
    {
        AssignPharmacist(pharmacistId);
        UpdateStatus(DiagnosticStatus.Collected);
        if (CompletedDate != completedDate) CompletedDate = completedDate;
    }
}
