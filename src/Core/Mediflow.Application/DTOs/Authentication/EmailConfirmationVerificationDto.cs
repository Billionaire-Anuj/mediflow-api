namespace Mediflow.Application.DTOs.Authentication;

public class EmailConfirmationVerificationDto
{
    public string EmailAddressOrUsername { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;
}
