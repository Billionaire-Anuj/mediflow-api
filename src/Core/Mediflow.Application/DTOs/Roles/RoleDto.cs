using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Roles;

public class RoleDto : BaseDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsDisplayed { get; set; }

    public bool IsRegisterable { get; set; }
}