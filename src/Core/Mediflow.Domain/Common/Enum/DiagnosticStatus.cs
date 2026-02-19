namespace Mediflow.Domain.Common.Enum;

public enum DiagnosticStatus
{
    Appointed,
    Scheduled,
    Cancelled,
    // This is exclusively only for Appointment Diagnostics
    Resulted,
    // This is exclusively only for Appointment Medications
    Collected
}