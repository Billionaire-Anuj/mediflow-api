using Mediflow.Application.Common.Authorization;

namespace Mediflow.API.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class PermissionAttribute : Microsoft.AspNetCore.Authorization.AuthorizeAttribute
{
    public PermissionAttribute(string resources, string action)
    {
        Policy = MediflowPermission.NameFor(resources, action);
    }
}