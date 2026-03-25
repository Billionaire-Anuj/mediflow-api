using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Notifications;

public static class NotificationExtensionMethods
{
    public static NotificationDto ToNotificationDto(this Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            IsActive = notification.IsActive,
            UserId = notification.UserId,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            ActionUrl = notification.ActionUrl,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }
}
