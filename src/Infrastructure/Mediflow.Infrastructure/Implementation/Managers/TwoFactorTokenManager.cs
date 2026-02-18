using OtpNet;
using Mediflow.Application.Interfaces.Managers;

namespace Mediflow.Infrastructure.Implementation.Managers;

public class TwoFactorTokenManager : ITwoFactorTokenManager
{
    public bool ValidateCode(string secretKey, string code)
    {
        var bytes = Base32Encoding.ToBytes(secretKey);

        var totp = new Totp(bytes, step: 30, mode: OtpHashMode.Sha1, totpSize: 6);

        return totp.VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));
    }
}