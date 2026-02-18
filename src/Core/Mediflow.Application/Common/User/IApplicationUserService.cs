using System.Security.Claims;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Common.User;

public interface IApplicationUserService : ITransientService
{
    bool IsAuthenticated { get; }

    Guid GetUserId { get; }

    string GetUserEmail { get; }

    string GetUserRole { get; }

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();
}