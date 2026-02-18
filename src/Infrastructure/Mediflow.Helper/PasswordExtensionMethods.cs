using System.Text;
using System.Security.Cryptography;

namespace Mediflow.Helper;

public static class PasswordExtensionMethods
{
    #region Hashing and Verification
    private const char SegmentDelimiter = ':';

    extension(string input)
    {
        public string Hash()
        {
            const int keySize = 32;
            const int saltSize = 16;
            const int iterations = 100_000;
        
            var algorithm = HashAlgorithmName.SHA256;
            var salt = RandomNumberGenerator.GetBytes(saltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

            return string.Join(
                SegmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                iterations,
                algorithm
            );
        }

        public bool VerifyHash(string hashString)
        {
            var segments = hashString.Split(SegmentDelimiter);
        
            var hash = Convert.FromHexString(segments[0]);
        
            var salt = Convert.FromHexString(segments[1]);
        
            var iterations = int.Parse(segments[2]);
        
            var algorithm = new HashAlgorithmName(segments[3]);
        
            var inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                iterations,
                algorithm,
                hash.Length
            );

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
    }
    #endregion

    #region Generation
    private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Numbers = "0123456789";
    private const string Special = "!@#$%^&*()-_=+[]{}|;:,.<>?/";

    public static string GeneratePassword(int length = 8, bool includeLowercase = true, bool includeUppercase = true, bool includeNumbers = true, bool includeSpecial = true)
    {
        var charPool = new StringBuilder();

        if (includeLowercase) charPool.Append(Lowercase);
        if (includeUppercase) charPool.Append(Uppercase);
        if (includeNumbers) charPool.Append(Numbers);
        if (includeSpecial) charPool.Append(Special);

        if (charPool.Length == 0)
            throw new ArgumentException("At least one character set must be included.");

        var random = new Random();
        var password = new char[length];

        for (var i = 0; i < length; i++)
        {
            password[i] = charPool[random.Next(charPool.Length)];
        }

        return new string(password);
    }
    #endregion
}