using Mediflow.Domain.Common.Enum;
using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class UserLoginLog(
    Guid? userId,
    string emailAddressOrUsername,
    LoginEventType eventType,
    LoginStatus status,
    string? accessToken,
    string? ipAddress,
    string? userAgent,
    bool isActiveSession
) : BaseEntity<Guid>
{
    [ForeignKey(nameof(User))]
    public Guid? UserId { get; private set; } = userId;

    public string EmailAddressOrUsername { get; private set; } = emailAddressOrUsername;

    public LoginEventType EventType { get; private set; } = eventType;

    public LoginStatus Status { get; private set; } = status;

    /// <summary>
    /// The access token used for this session and is only set up for successful login.
    /// </summary>
    public string? AccessToken { get; private set; } = accessToken;

    public string? IpAddress { get; private set; } = ipAddress;

    public string? UserAgent { get; private set; } = userAgent;

    /// <summary>
    /// Defines a true flag when this log represents the currently active session for the user.
    /// </summary>
    public bool IsActiveSession { get; private set; } = isActiveSession;

    public DateTime ActionDate { get; private set; } = DateTime.Now;

    public DateTime? LoggedOutDate { get; private set; }

    public virtual User User { get; set; } = User.Default;

    public void MarkLoggedOut(bool forced)
    {
        IsActiveSession = false;
        LoggedOutDate = DateTime.Now;
        EventType = LoginEventType.Logout;
        Status = forced ? LoginStatus.ForcedLogout : LoginStatus.LoggedOut;
    }
}