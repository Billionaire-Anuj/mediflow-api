namespace Mediflow.Application.DTOs.Emails;

public class ResetPasswordEmailDto
{
    public Guid UserId { get; set; }

    public Guid ApplicationUserId { get; set; }

    public string Password { get; set; } = string.Empty;
}