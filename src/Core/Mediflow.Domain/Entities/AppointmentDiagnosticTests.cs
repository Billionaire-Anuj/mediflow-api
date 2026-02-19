using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class AppointmentDiagnosticTests(Guid appointmentDiagnosticsId, Guid diagnosticTestId) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(AppointmentDiagnostics))]
    public Guid AppointmentDiagnosticsId { get; private set; } = appointmentDiagnosticsId;

    [ForeignKey(nameof(DiagnosticTest))]
    public Guid DiagnosticTestId { get; private set; } = diagnosticTestId;

    /// <summary>
    /// This is for special cases such as X-Rays, MRIs or Imaging Tests.
    /// </summary>
    public Asset? DiagnosticReport { get; private set; }

    public static AppointmentDiagnosticTests Default => new(Guid.Empty, Guid.Empty);

    /// <summary>
    /// This is when the lab technician provides results for the diagnostic test.
    /// It includes blood test results, imaging findings, or other relevant information.
    /// </summary>
    public virtual AppointmentDiagnosticTestResult? AppointmentDiagnosticTestResult { get; set; }

    public virtual AppointmentDiagnostics? AppointmentDiagnostics { get; set; }

    public virtual DiagnosticTest? DiagnosticTest { get; set; }

    public void AttachDiagnosticReport(Asset diagnosticReport)
    {
        DiagnosticReport = diagnosticReport;
    }
}
