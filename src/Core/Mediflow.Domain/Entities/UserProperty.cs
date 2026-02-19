using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Property;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class UserProperty(Guid userId, KeyValueProperty configurations) : BaseEntity<Guid>
{
    [ForeignKey(nameof(User))] 
    public Guid UserId { get; private set; } = userId;

    public KeyValueProperty Configurations { get; private set; } = configurations;

    public virtual User User { get; set; } = User.Default;

    public void UpdateConfigurations(KeyValueProperty configurations)
    {
        Configurations = configurations ?? throw new ArgumentNullException(nameof(configurations), "Configurations cannot be null.");
    }
}