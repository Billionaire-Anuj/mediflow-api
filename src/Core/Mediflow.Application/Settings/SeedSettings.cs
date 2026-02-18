using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Settings;

public class SeedSettings : IValidatableObject
{
    public UserSeedSettings SuperAdmin { get; set; } = new();

    public UserSeedSettings TenantAdministrator { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }
}

public class UserSeedSettings : IValidatableObject
{
    public string Identifier { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Identifier) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Username) || 
            string.IsNullOrEmpty(EmailAddress) || string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Password))
        {
            yield return new ValidationResult(
                $"{nameof(SeedSettings)} is not configured.",
                [
                    nameof(Identifier),
                    nameof(Name),
                    nameof(Username),
                    nameof(EmailAddress),
                    nameof(Address),
                    nameof(PhoneNumber),
                    nameof(Password)
                ]);
        }
    }
}