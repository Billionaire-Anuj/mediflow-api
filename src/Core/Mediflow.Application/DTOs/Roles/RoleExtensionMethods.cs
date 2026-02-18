using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Roles;

public static class RoleExtensionMethods
{
    public static RoleDto ToRoleDto(this Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            IsActive = role.IsActive,
            Name = role.Name,
            Description = role.Description,
            IsDisplayed = role.IsDisplayed,
            IsRegisterable = role.IsRegisterable
        };
    }
}