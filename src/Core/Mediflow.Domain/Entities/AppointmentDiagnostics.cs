using Mediflow.Domain.Common.Enum;
using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class AppointmentDiagnostics(
    Guid appointmentId,
    Guid? labTechnicianId,
    string notes,
    DiagnosticStatus status = DiagnosticStatus.Appointed) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Appointment))]
    public Guid AppointmentId { get; private set; } = appointmentId;

    [ForeignKey(nameof(LabTechnician))]
    public Guid? LabTechnicianId { get; private set; } = labTechnicianId;

    public string Notes { get; private set; } = notes;

    public DiagnosticStatus Status { get; private set; } = status;

    public DateTime? CompletedDate { get; private set; }

    public static AppointmentDiagnostics Default => new(Guid.Empty, null, string.Empty);

    public virtual Appointment? Appointment { get; set; }

    public virtual User? LabTechnician { get; set; }

    public virtual ICollection<AppointmentDiagnosticTests> DiagnosticTests { get; set; } = new List<AppointmentDiagnosticTests>();

    public void AssignLabTechnician(Guid? labTechnicianId)
    {
        if (LabTechnicianId != labTechnicianId) LabTechnicianId = labTechnicianId;
    }

    public void UpdateNotes(string notes)
    {
        if (Notes != notes) Notes = notes;
    }

    public void UpdateStatus(DiagnosticStatus status)
    {
        if (Status != status) Status = status;
    }

    public void MarkCompleted(DateTime completedDate, DiagnosticStatus status)
    {
        UpdateStatus(status);
        if (CompletedDate != completedDate) CompletedDate = completedDate;
    }
}
