namespace Mediflow.Application.DTOs.Authentication;

public class ForgotPasswordResetDto : ForgotPasswordConfirmationDto
{
    public string Otp { get; set; } = string.Empty;

    public string Password {  get; set; } = string.Empty;
}
