using Mediflow.Domain.Common.Base;

namespace Mediflow.Domain.Entities;

public class Specialization(string title, string description) : AuditableEntity<Guid>
{
    public string Title { get; private set; } = title;

    public string Description { get; private set; } = description;

    public virtual ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();

    public static Specialization Default => new(string.Empty, string.Empty);

    public void Update(string title, string description)
    {
        if (Title != title) Title = title;
        if (Description != description) Description = description;
    }
}