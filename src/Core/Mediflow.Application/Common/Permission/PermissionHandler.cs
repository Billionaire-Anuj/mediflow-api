using Microsoft.AspNetCore.Authorization;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Application.Common.Permission;

public sealed class PermissionHandler(IPermissionService permissionService) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var hasPermission = permissionService.HasPermission(requirement.Action, requirement.Resource);

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
