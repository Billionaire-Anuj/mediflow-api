namespace Mediflow.Application.DTOs.Authentication.Configurations._2FA;

public class TwoFactorSetupDto
{
    public string QrCodeImageBase64 { get; set; } = string.Empty;

    public string ManualEntryKey { get; set; } = string.Empty;
}