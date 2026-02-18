namespace Mediflow.Application.DTOs.Authentication.Configurations._2FA;

public abstract class AbstractTwoFactorAuthenticationConfiguration
{
    public string SecretKey { get; set; } = string.Empty;

    public bool IsConfirmed { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}