using Mediflow.Domain.Common.Base;

namespace Mediflow.Domain.Entities;

public class Resource(string name, string description) : BaseEntity<Guid>
{
    public string Name { get; private set; } = name;  

    public string Description { get; private set; } = description; 

    public virtual ICollection<Permission>? Permissions { get; set; }
}