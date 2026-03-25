using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Notifications;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class NotificationService(
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService) : INotificationService
{
    private static readonly string[] AdministrativeRoleNames =
    [
        Constants.Roles.SuperAdmin.Name,
        Constants.Roles.TenantAdministrator.Name,
        "Admin"
    ];

    public List<NotificationDto> GetMyNotifications(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        NotificationType[]? types = null,
        bool? isRead = null)
    {
        var userId = applicationUserService.GetUserId;
        var typeIdentifiers = types != null ? new HashSet<NotificationType>(types) : null;

        var notificationModels = applicationDbContext.Notifications
            .Where(x =>
                x.UserId == userId &&
                (string.IsNullOrEmpty(globalSearch)
                    || x.Title.ToLower().Contains(globalSearch.Trim().ToLower())
                    || x.Message.ToLower().Contains(globalSearch.Trim().ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (typeIdentifiers == null || typeIdentifiers.Contains(x.Type)) &&
                (isRead == null || x.IsRead == isRead.Value))
            .OrderBy(x => orderBys);

        var results = notificationModels.Select(x => x.ToNotificationDto()).ToList();

        if (orderBys == null || orderBys.Length == 0)
        {
            results = results.OrderByDescending(x => x.CreatedAt).ToList();
        }

        return results;
    }

    public void MarkAsRead(Guid notificationId)
    {
        var userId = applicationUserService.GetUserId;

        var notification = applicationDbContext.Notifications
            .FirstOrDefault(x => x.Id == notificationId && x.UserId == userId)
            ?? throw new NotFoundException($"Notification with identifier '{notificationId}' was not found.");

        notification.MarkAsRead(DateTime.Now);
        applicationDbContext.SaveChanges();
    }

    public void MarkAllAsRead()
    {
        var userId = applicationUserService.GetUserId;

        var unreadNotifications = applicationDbContext.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .ToList();

        if (unreadNotifications.Count == 0)
        {
            return;
        }

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead(DateTime.Now);
        }

        applicationDbContext.SaveChanges();
    }

    public void QueueNotification(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string actionUrl,
        string? notificationKey = null)
    {
        var sanitizedKey = string.IsNullOrWhiteSpace(notificationKey)
            ? Guid.NewGuid().ToString("N")
            : notificationKey.Trim();

        var exists = applicationDbContext.Notifications.Local.Any(x => x.UserId == userId && x.NotificationKey == sanitizedKey)
                     || applicationDbContext.Notifications.Any(x => x.UserId == userId && x.NotificationKey == sanitizedKey);

        if (exists)
        {
            return;
        }

        var notification = new Notification(
            userId,
            type,
            title.Trim(),
            message.Trim(),
            actionUrl.Trim(),
            sanitizedKey);

        applicationDbContext.Notifications.Add(notification);
    }

    public void QueueNotifications(
        IEnumerable<Guid> userIds,
        NotificationType type,
        string title,
        string message,
        string actionUrl,
        string? notificationKey = null)
    {
        foreach (var userId in userIds.Where(x => x != Guid.Empty).Distinct())
        {
            var keyed = notificationKey is null ? null : $"{notificationKey}:{userId:N}";
            QueueNotification(userId, type, title, message, actionUrl, keyed);
        }
    }

    public void QueueForAdministrativeUsers(
        NotificationType type,
        string title,
        string message,
        string actionUrl,
        string? notificationKey = null)
    {
        var adminUserIds = applicationDbContext.Users
            .Where(x =>
                x.IsActive &&
                x.Role != null &&
                AdministrativeRoleNames.Contains(x.Role.Name))
            .Select(x => x.Id)
            .ToList();

        QueueNotifications(adminUserIds, type, title, message, actionUrl, notificationKey);
    }
}
