using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Permissions;

namespace Mediflow.Application.Interfaces.Services;

public interface IPermissionService : IScopedService
{
    bool HasPermission(string action, string resource); 

    List<AssignedPermissionsDto> GetAssignedPermissions();

    List<AssignedPermissionsDto> GetAllocatedPermissions(Guid roleId);
    
    void GrantPermission(GrantPermissionsDto grantPermissions);
}