using Mediflow.Application.Common.User;
using Mediflow.Domain.Common.Enum;
using Mediflow.Infrastructure.Implementation.Services;
using Mediflow.Tests.Common;
using Moq;

namespace Mediflow.Tests.Services;

public class NotificationServiceTests
{
    [Fact]
    public void QueueNotification_DeduplicatesPerUserAndMarkAllAsReadTargetsCurrentUserOnly()
    {
        using var context = TestApplicationDbContextFactory.Create();

        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var appUserService = new Mock<IApplicationUserService>();
        appUserService.SetupGet(x => x.GetUserId).Returns(currentUserId);

        var service = new NotificationService(context, appUserService.Object);

        service.QueueNotification(currentUserId, NotificationType.Appointment, "Booked", "Booked message", "/patient/appointments/1", "same-key");
        service.QueueNotification(currentUserId, NotificationType.Appointment, "Booked", "Duplicate should be ignored", "/patient/appointments/1", "same-key");
        service.QueueNotification(otherUserId, NotificationType.Lab, "Lab", "Other user message", "/lab/request/1", "same-key");
        context.SaveChanges();

        var mine = service.GetMyNotifications();

        Assert.Single(mine);
        Assert.Equal("Booked", mine[0].Title);

        service.MarkAllAsRead();

        var currentUserNotifications = context.Notifications.Where(x => x.UserId == currentUserId).ToList();
        var otherUserNotifications = context.Notifications.Where(x => x.UserId == otherUserId).ToList();

        Assert.All(currentUserNotifications, notification => Assert.True(notification.IsRead));
        Assert.All(otherUserNotifications, notification => Assert.False(notification.IsRead));
    }
}
