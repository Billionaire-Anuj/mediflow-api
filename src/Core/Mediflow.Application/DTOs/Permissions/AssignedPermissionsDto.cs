using Mediflow.Application.DTOs.Resources;

namespace Mediflow.Application.DTOs.Permissions;

public class AssignedPermissionsDto : ResourceDto
{
    public List<PermissionDto> Permissions { get; set; } = [];
}