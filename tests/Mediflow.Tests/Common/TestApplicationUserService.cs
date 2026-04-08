using System.Security.Claims;
using Mediflow.Application.Common.User;

namespace Mediflow.Tests.Common;

internal sealed class TestApplicationUserService(Guid userId, string email = "test@mediflow.local", string role = "Test") : IApplicationUserService
{
    public bool IsAuthenticated => true;

    public Guid GetUserId => userId;

    public string GetUserEmail => email;

    public string GetUserRole => role;

    public bool IsInRole(string roleName) => string.Equals(role, roleName, StringComparison.OrdinalIgnoreCase);

    public IEnumerable<Claim>? GetUserClaims() => [];
}
