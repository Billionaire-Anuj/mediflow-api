using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class Permission(Guid roleId, Guid resourceId, string action) : BaseEntity<Guid>
{
    [ForeignKey(nameof(Role))] 
    public Guid RoleId { get; set; } = roleId;
    
    [ForeignKey(nameof(Resource))]
    public Guid ResourceId { get; set; } = resourceId;
    
    public string Action { get; set; } = action;
    
    public virtual Resource Resource { get; set; } = Resource.Default;

    public virtual Role Role { get; set; } = Role.Default;
}