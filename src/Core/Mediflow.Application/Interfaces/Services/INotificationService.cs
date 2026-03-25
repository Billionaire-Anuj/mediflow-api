using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Notifications;

namespace Mediflow.Application.Interfaces.Services;

public interface INotificationService : IScopedService
{
    List<NotificationDto> GetMyNotifications(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        NotificationType[]? types = null,
        bool? isRead = null);

    void MarkAsRead(Guid notificationId);

    void MarkAllAsRead();

    void QueueNotification(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string actionUrl,
        string? notificationKey = null);

    void QueueNotifications(
        IEnumerable<Guid> userIds,
        NotificationType type,
        string title,
        string message,
        string actionUrl,
        string? notificationKey = null);

    void QueueForAdministrativeUsers(
        NotificationType type,
        string title,
        string message,
        string actionUrl,
        string? notificationKey = null);
}
