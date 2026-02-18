using Mediflow.Application.Interfaces.Managers;

namespace Mediflow.Infrastructure.Implementation.Managers;

public class TokenManager : ITokenManager
{
    private readonly HashSet<string> _blacklist = [];
    
    public void BlacklistToken(string token) => _blacklist.Add(token);
    
    public bool IsBlacklisted(string token) => _blacklist.Contains(token);
}