using System.ComponentModel.DataAnnotations.Schema;
using Mediflow.Domain.Common.Base;

namespace Mediflow.Domain.Entities;

public class DiagnosticTest(Guid diagnosticTypeId, string title, string description, string specimen) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(DiagnosticType))]
    public Guid DiagnosticTypeId { get; private set; } = diagnosticTypeId;

    public string Title { get; private set; } = title;
    
    public string Description { get; private set; } = description;

    /// <summary>
    /// Refers to the specimen required for the diagnostic test, such as blood, urine or tissue.
    /// </summary>
    public string Specimen { get; private set; } = specimen;

    public static DiagnosticTest Default => new(Guid.Empty, string.Empty, string.Empty, string.Empty);

    public virtual DiagnosticType? DiagnosticType { get; set; }

    public virtual ICollection<AppointmentDiagnosticTests> AppointmentDiagnosticTests { get; set; } = new List<AppointmentDiagnosticTests>();

    public void Update(Guid diagnosticTypeId, string title, string description, string specimen)
    {
        if (DiagnosticTypeId != diagnosticTypeId) DiagnosticTypeId = diagnosticTypeId;
        if (Title != title) Title = title;
        if (Description != description) Description = description;
        if (Specimen != specimen) Specimen = specimen;
    }
}