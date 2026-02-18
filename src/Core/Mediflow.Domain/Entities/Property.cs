using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Property;

namespace Mediflow.Domain.Entities;

public class Property(KeyValueProperty setting) : BaseEntity<Guid>
{
    #region Propeties
    public KeyValueProperty Setting { get; private set; } = setting;
    #endregion

    public void Update(KeyValueProperty setting)
    {
        Setting = setting;
    }
}