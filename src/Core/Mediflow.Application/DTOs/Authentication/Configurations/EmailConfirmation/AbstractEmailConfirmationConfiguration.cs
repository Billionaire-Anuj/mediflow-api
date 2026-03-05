namespace Mediflow.Application.DTOs.Authentication.Configurations.EmailConfirmation;

public abstract class AbstractEmailConfirmationConfiguration
{
    public string OneTimePassword { get; set; } = string.Empty;

    public bool IsVerified { get; set; }
}
