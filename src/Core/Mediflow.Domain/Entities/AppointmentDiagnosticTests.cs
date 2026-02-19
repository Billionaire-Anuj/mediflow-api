using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class AppointmentDiagnosticTests(Guid appointmentDiagnosticsId, Guid diagnosticTestId)
{
    [ForeignKey(nameof(AppointmentDiagnostics))]
    public Guid AppointmentDiagnosticsId { get; private set; } = appointmentDiagnosticsId;

    [ForeignKey(nameof(DiagnosticTest))]
    public Guid DiagnosticTestId { get; private set; } = diagnosticTestId;

    public static AppointmentDiagnosticTests Default => new(Guid.Empty, Guid.Empty);

    public virtual AppointmentDiagnosticTestResult AppointmentDiagnosticTestResult { get; set; } = AppointmentDiagnosticTestResult.Default;

    public virtual AppointmentDiagnostics AppointmentDiagnostics { get; set; } = AppointmentDiagnostics.Default;

    public virtual DiagnosticTest DiagnosticTest { get; set; } = DiagnosticTest.Default;
}