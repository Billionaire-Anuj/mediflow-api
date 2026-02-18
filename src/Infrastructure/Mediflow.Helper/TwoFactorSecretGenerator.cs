using OtpNet;

namespace Mediflow.Helper;

public static class TwoFactorSecretGenerator
{
    /// <summary>
    /// Generates a random Base32 secret suitable for TOTP.
    /// 20 bytes => 32 Base32 chars (typical length).
    /// </summary>
    /// <param name="numBytes">Number of random bytes.</param>
    public static string GenerateBase32Secret(int numBytes = 20)
    {
        var bytes = KeyGeneration.GenerateRandomKey(numBytes);
        var base32 = Base32Encoding.ToString(bytes);

        return base32.TrimEnd('=').ToUpperInvariant();
    }
}