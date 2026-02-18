using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Managers;

public interface ITokenManager : ISingletonService
{
    void BlacklistToken(string token);

    bool IsBlacklisted(string token);
}