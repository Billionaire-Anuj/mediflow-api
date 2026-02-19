using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class AppointmentDiagnosticTests(Guid appointmentDiagnosticsId, Guid diagnosticTestId) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(AppointmentDiagnostics))]
    public Guid AppointmentDiagnosticsId { get; private set; } = appointmentDiagnosticsId;

    [ForeignKey(nameof(DiagnosticTest))]
    public Guid DiagnosticTestId { get; private set; } = diagnosticTestId;

    public static AppointmentDiagnosticTests Default => new(Guid.Empty, Guid.Empty);

    public virtual AppointmentDiagnosticTestResult? AppointmentDiagnosticTestResult { get; set; }

    public virtual AppointmentDiagnostics? AppointmentDiagnostics { get; set; }

    public virtual DiagnosticTest? DiagnosticTest { get; set; }
}