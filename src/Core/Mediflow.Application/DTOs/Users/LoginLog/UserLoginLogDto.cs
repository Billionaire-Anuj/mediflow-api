using Mediflow.Domain.Common.Enum;

namespace Mediflow.Application.DTOs.Users.LoginLog;

public class UserLoginLogDto
{
    public UserDto? User { get; set; }

    public string EmailAddressOrUsername { get; set; } = string.Empty;

    public LoginEventType EventType { get; set; }

    public LoginStatus Status { get; set; }

    public string? AccessToken { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public bool IsActiveSession { get; set; }

    public DateTime ActionDate { get; set; }

    public DateTime? LoggedOutDate { get; set; }
}