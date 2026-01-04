using System.Security.Claims;

namespace mediflow.Infrastructure;

public static class UserContext
{
    public static Guid? GetUserId(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : null;
    }

    public static bool IsAdmin(ClaimsPrincipal user) => user.IsInRole("Admin");
}