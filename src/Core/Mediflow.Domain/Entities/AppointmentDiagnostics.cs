using Mediflow.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class AppointmentDiagnostics(Guid appointmentId, Guid? labTechnicianId, string notes, DiagnosticStatus status = DiagnosticStatus.Appointed)
{
    [ForeignKey(nameof(Appointment))]
    public Guid AppointmentId { get; private set; } = appointmentId;

    [ForeignKey(nameof(LabTechnician))]
    public Guid? LabTechnicianId { get; private set; } = labTechnicianId;

    public string Notes { get; private set; } = notes;

    public DiagnosticStatus Status { get; private set; } = status;

    public DateTime? CompletedDate { get; private set; }

    public static AppointmentDiagnostics Default => new(Guid.Empty, null, string.Empty);

    public virtual Appointment Appointment { get; set; } = Appointment.Default;

    public virtual User LabTechnician { get; set; } = User.Default;

    public virtual ICollection<AppointmentDiagnosticTests> DiagnosticTests { get; set; } = new List<AppointmentDiagnosticTests>();

    public void AssignLabTechnician(Guid? labTechnicianId)
    {
        if (LabTechnicianId != labTechnicianId) LabTechnicianId = labTechnicianId;
    }
}