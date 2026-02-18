namespace Mediflow.Application.DTOs.Emails;

public class ForgotPasswordEmailDto
{
    public Guid UserId { get; set; }

    public string Otp { get; set; } = string.Empty;

    public int OtpExpiryMinutes { get; set; }
}