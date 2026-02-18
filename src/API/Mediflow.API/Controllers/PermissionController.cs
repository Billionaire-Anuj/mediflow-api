using Mediflow.Domain.Common;
using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Permissions;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class PermissionController(IPermissionService permissionService) : BaseController<PermissionController>
{
    [HttpGet]
    [Documentation("GetAssignedPermissions", "Retrieve the assigned permissions based on the active user's role.")]
    public ResponseDto<List<AssignedPermissionsDto>> GetAssignedPermission()
    {
        var result = permissionService.GetAssignedPermissions();

        return new ResponseDto<List<AssignedPermissionsDto>>(
            (int)HttpStatusCode.OK,
            "Successfully fetched assigned permission based on their role.",
            result);
    }

    [HttpGet("{roleId:guid}")]
    [Authorize(Constants.Roles.SuperAdmin.Name, Constants.Roles.TenantAdministrator.Name)]
    [Documentation("GetAllocatedPermissions", "Retrieve the allocated permissions based on their role.")]
    public ResponseDto<List<AssignedPermissionsDto>> GetAllocatedPermissions([FromRoute] Guid roleId)
    {
        var result = permissionService.GetAllocatedPermissions(roleId);

        return new ResponseDto<List<AssignedPermissionsDto>>(
            (int)HttpStatusCode.OK,
            "Successfully fetched assigned permission based on their role.",
            result);
    }
    
    [HttpPost]
    [Authorize(Constants.Roles.SuperAdmin.Name, Constants.Roles.TenantAdministrator.Name)]
    [Documentation("GrantPermissions", "Grant and revoke permission access to a specified role.")]
    public ResponseDto<bool> GrantPermissions([FromBody] GrantPermissionsDto permissions)
    {
        permissionService.GrantPermission(permissions);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Permissions successfully requested.",
            true
        );
    }
}