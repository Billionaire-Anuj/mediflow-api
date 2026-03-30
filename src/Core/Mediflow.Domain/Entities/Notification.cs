using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class Notification(
    Guid userId,
    NotificationType type,
    string title,
    string message,
    string actionUrl,
    string notificationKey,
    bool isRead = false,
    DateTime? readAt = null
) : BaseEntity<Guid>
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; private set; } = userId;

    public NotificationType Type { get; private set; } = type;

    public string Title { get; private set; } = title;

    public string Message { get; private set; } = message;

    public string ActionUrl { get; private set; } = actionUrl;

    public string NotificationKey { get; private set; } = notificationKey;

    public bool IsRead { get; private set; } = isRead;

    public DateTime? ReadAt { get; private set; } = readAt;

    public virtual User? User { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public static Notification Default => new(Guid.Empty, NotificationType.System, string.Empty, string.Empty, string.Empty, string.Empty);

    public void MarkAsRead(DateTime readDate)
    {
        if (IsRead)
        {
            return;
        }

        IsRead = true;
        ReadAt = readDate;
    }
}
