using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class AppointmentMedicationDrugs(
    Guid appointmentMedicationsId,
    Guid medicineId,
    string dose,
    string frequency,
    int duration,
    string? instructions = null) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(AppointmentMedications))]
    public Guid AppointmentMedicationsId { get; private set; } = appointmentMedicationsId;

    [ForeignKey(nameof(Medicine))]
    public Guid MedicineId { get; private set; } = medicineId;

    /// <summary>
    /// Defines the dosage of the drug, such as "500mg" or "10ml".
    /// </summary>
    public string Dose { get; private set; } = dose;

    /// <summary>
    /// Defines how often the drug should be taken, such as "twice a day" or "every 8 hours".
    /// </summary>
    public string Frequency { get; private set; } = frequency;

    /// <summary>
    /// Defines the duration for which the drug should be taken, such as "5 days" or "2 weeks".
    /// </summary>
    public int Duration { get; private set; } = duration;

    /// <summary>
    /// Defines any additional instructions for taking the drug, such as "take with food" or "before sleep".
    /// </summary>
    public string? Instructions { get; private set; } = instructions;

    public static AppointmentMedicationDrugs Default => new(Guid.Empty, Guid.Empty, string.Empty, string.Empty, 0);

    public virtual AppointmentMedications? AppointmentMedications { get; set; }

    public virtual Medicine? Medicine { get; set; }
}