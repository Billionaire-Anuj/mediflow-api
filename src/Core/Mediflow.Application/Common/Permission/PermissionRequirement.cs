using Microsoft.AspNetCore.Authorization;

namespace Mediflow.Application.Common.Permission;

public sealed class PermissionRequirement(string resource, string action) : IAuthorizationRequirement
{
    public string Resource { get; } = resource;

    public string Action { get; } = action;
}