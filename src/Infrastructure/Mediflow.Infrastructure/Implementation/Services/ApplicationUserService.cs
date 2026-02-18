using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;

namespace Mediflow.Infrastructure.Implementation.Services;

public class ApplicationUserService(IHttpContextAccessor contextAccessor) : IApplicationUserService
{
    public bool IsAuthenticated
    {
        get
        {
            var userId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            return userId != null;
        }
    }
    
    public Guid GetUserId
    {
        get
        {
            var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdClaimValue?.Value, out var userId) ? userId : Guid.Empty;
        }
    }

    public string GetUserEmail
    {
        get
        {
            var emailAddressClaimValue = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email);

            return emailAddressClaimValue?.Value ?? throw new NotFoundException("The respective user is not authenticated.");
        }
    }

    public string GetUserRole
    {
        get
        {
            var roleClaimValue = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

            return roleClaimValue?.Value ?? throw new NotFoundException("The respective user is not authenticated.");
        }
    }

    public bool IsInRole(string role)
    {
        var roleName = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

        return roleName != null && roleName.Value == role;
    }

    public IEnumerable<Claim> GetUserClaims()
    {
        var claims = contextAccessor.HttpContext?.User.Claims;
        
        return claims ?? [];
    }
}