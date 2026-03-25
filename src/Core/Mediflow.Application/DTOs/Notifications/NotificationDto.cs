using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Notifications;

public class NotificationDto : BaseDto
{
    public Guid UserId { get; set; }

    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string ActionUrl { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
