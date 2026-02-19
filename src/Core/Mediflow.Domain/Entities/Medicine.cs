using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class Medicine(Guid medicationTypeId, string title, string description, string format) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(MedicationType))]
    public Guid MedicationTypeId { get; private set; } = medicationTypeId;

    public string Title { get; private set; } = title;

    public string Description { get; private set; } = description;

    /// <summary>
    /// Refers to the format of the drug, such as syrup, tablets or injections.
    /// </summary>
    public string Format { get; private set; } = format;

    public static Medicine Default => new(Guid.Empty, string.Empty, string.Empty, string.Empty);

    public virtual MedicationType? MedicationType { get; set; }

    public virtual ICollection<AppointmentMedicationDrugs> AppointmentMedicationDrugs { get; set; } = new List<AppointmentMedicationDrugs>();
}