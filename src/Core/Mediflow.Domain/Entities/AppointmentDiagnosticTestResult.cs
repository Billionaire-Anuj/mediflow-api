using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class AppointmentDiagnosticTestResult(
    Guid appointmentDiagnosticTestId,
    string value,
    string unit,
    string upperRange,
    string lowerRange,
    string interpretation) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(AppointmentDiagnosticTests))]
    public Guid AppointmentDiagnosticTestId { get; private set; } = appointmentDiagnosticTestId;

    /// <summary>
    /// Defines the actual result value of the diagnostic test.
    /// It can be a numeric measurement, a qualitative description, or a categorical outcome.
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Defines the unit of measurement for the test result, such as "mg/dL", "mmHg", "bpm", or "positive/negative".
    /// </summary>
    public string Unit { get; private set; } = unit;

    /// <summary>
    /// Defines the upper limit of the normal reference range for the test result.
    /// </summary>
    public string UpperRange { get; private set; } = upperRange;

    /// <summary>
    /// Defines the lower limit of the normal reference range for the test result.
    /// </summary>
    public string LowerRange { get; private set; } = lowerRange;

    /// <summary>
    /// Defines the interpretation of the test result, such as "Normal", "High", "Low", "Abnormal", or specific clinical implications based on the value and reference ranges.
    /// </summary>
    public string Interpretation { get; private set; } = interpretation;

    public static AppointmentDiagnosticTestResult Default => new(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

    public virtual AppointmentDiagnosticTests? AppointmentDiagnosticTests { get; set; }

    public void Update(string value, string unit, string upperRange, string lowerRange, string interpretation)
    {
        if (Value != value) Value = value;
        if (Unit != unit) Unit = unit;
        if (UpperRange != upperRange) UpperRange = upperRange;
        if (LowerRange != lowerRange) LowerRange = lowerRange;
        if (Interpretation != interpretation) Interpretation = interpretation;
    }
}
