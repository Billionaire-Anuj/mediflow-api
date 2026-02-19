using Mediflow.Domain.Common.Base;

namespace Mediflow.Domain.Entities;

public class MedicationType(string title, string description) : AuditableEntity<Guid>
{
    public string Title { get; set; } = title;
    
    public string Description { get; set; } = description;

    public static MedicationType Default => new(string.Empty, string.Empty);

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();

    public void Update(string title, string description)
    {
        if (Title != title) Title = title;
        if (Description != description) Description = description;
    }
}