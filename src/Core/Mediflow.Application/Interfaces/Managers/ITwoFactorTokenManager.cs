using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Managers;

public interface ITwoFactorTokenManager : IScopedService
{
    /// <summary>
    /// Validates a TOTP code for the given Base32 secret key.
    /// </summary>
    bool ValidateCode(string secretKey, string code);
}