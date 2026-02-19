using Mediflow.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using Mediflow.Domain.Common.Base;

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

    public virtual Appointment Appointment { get; set; } = Appointment.Default;

    public virtual User Pharmacist { get; set; } = User.Default;

    public virtual ICollection<AppointmentMedicationDrugs> Drugs { get; set; } = new List<AppointmentMedicationDrugs>();

    public void AssignPharmacist(Guid? pharmacistId)
    {
        if (PharmacistId != pharmacistId) PharmacistId = pharmacistId;
    }
}