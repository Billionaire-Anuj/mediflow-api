namespace Mediflow.Application.DTOs.Emails;

public class SecurityNotificationEmailDto
{
    public Guid UserId { get; set; }

    public string Message { get; set; } = string.Empty;
}
