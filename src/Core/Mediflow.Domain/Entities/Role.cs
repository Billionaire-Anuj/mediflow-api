using Mediflow.Domain.Common.Base;

namespace Mediflow.Domain.Entities;

public class Role(string name, string description, bool isDisplayed, bool isRegisterable): BaseEntity<Guid>
{
    public string Name { get; private set; } = name;

    public string Description { get; private set; } = description;

    // The following property is used to determine whether the role is displayed in the UI and will not be exposed to the REST API.
    public bool IsDisplayed { get; private set; } = isDisplayed;

    // The following property is used to determine whether the role can be registered as a user role or not and will not be exposed to the REST API.
    public bool IsRegisterable { get; private set; } = isRegisterable;

    public virtual ICollection<User>? Users { get; set; }

    public virtual ICollection<Permission>? Permissions { get; set; }

    public static Role Default { get; } = new(string.Empty, string.Empty, false, false);

    public void Update(string name, string description, bool isDisplayed, bool isRegisterable)
    {
        if (Name != name) Name = name;
        if (Description != description) Description = description;
        if (IsDisplayed != isDisplayed) IsDisplayed = isDisplayed;
        if (IsRegisterable != isRegisterable) IsRegisterable = isRegisterable;
    }
}