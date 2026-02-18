using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Settings;

public class SmtpSettings : IValidatableObject
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Host) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || Port == 0)
        {
            yield return new ValidationResult(
                $"{nameof(SmtpSettings)} is not configured.",
                [nameof(Host), nameof(Username), nameof(Password), nameof(Port)]);
        }
    }
}