using Mediflow.Domain.Common.Base;

namespace Mediflow.Domain.Entities;

public class DiagnosticType(string title, string description) : AuditableEntity<Guid>
{
    public string Title { get; set; } = title;
    
    public string Description { get; set; } = description;

    public static DiagnosticType Default => new(string.Empty, string.Empty);

    public virtual ICollection<DiagnosticTest> DiagnosticTests { get; set; } = new List<DiagnosticTest>();

    public void Update(string title, string description)
    {
        if (Title != title) Title = title;
        if (Description != description) Description = description;
    }
}