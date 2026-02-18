using Mediflow.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Mediflow.Application.Common.User;
using Microsoft.AspNetCore.Authorization;
using Mediflow.Application.Interfaces.Managers;

namespace Mediflow.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class AuthorizeAttribute(params string[] roles) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var endpoint = context.ActionDescriptor.EndpointMetadata;

        if (endpoint.Any(x => x is AllowAnonymousAttribute)) return;

        var accessToken = context.HttpContext.Request.Headers.Authorization.ToString();

        var token = accessToken.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedException("Authorization token is missing. Please try again by logging in.");
        }

        var tokenManager = context.HttpContext.RequestServices.GetRequiredService<ITokenManager>();

        if (tokenManager.IsBlacklisted(token))
        {
            throw new UnauthorizedException("Session has expired. Please log in again.");
        }

        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        if (roles.Length > 0)
        {
            var applicationUserService =
                context.HttpContext.RequestServices.GetRequiredService<IApplicationUserService>();

            var hasRequiredRole = roles.Any(role => applicationUserService.IsInRole(role));

            if (!hasRequiredRole)
            {
                throw new UnauthorizedException("You are not authorized to access this resource.");
            }
        }
    }
}
