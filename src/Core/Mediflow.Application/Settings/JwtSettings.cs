using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Settings;

public class JwtSettings : IValidatableObject
{
    public string Key { get; set; } = string.Empty;
    
    public string Issuer { get; set; } = string.Empty;
        
    public string Audience { get; set; } = string.Empty;
    
    public double AccessTokenExpirationInMinutes { get; set; }

    public double RefreshTokenExpirationInDays { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Issuer) || string.IsNullOrEmpty(Audience))
        {
            yield return new ValidationResult(
                $"{nameof(JwtSettings)} is not configured.",
                [nameof(Key), nameof(Issuer), nameof(Audience)]);
        }
    }
}