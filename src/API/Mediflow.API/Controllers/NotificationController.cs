using Mediflow.API.Attributes;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Notifications;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Domain.Common.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Mediflow.API.Controllers;

public class NotificationController(INotificationService notificationService) : BaseController<NotificationController>
{
    [HttpGet("list")]
    [Documentation("GetMyNotifications", "Retrieve all notifications for the logged in user.")]
    public ResponseDto<List<NotificationDto>> GetMyNotifications(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] NotificationType[]? types = null,
        [FromQuery] bool? isRead = null)
    {
        var result = notificationService.GetMyNotifications(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            types,
            isRead);

        return new ResponseDto<List<NotificationDto>>(
            (int)HttpStatusCode.OK,
            "Notifications successfully fetched.",
            result);
    }

    [HttpPatch("{notificationId:guid}/read")]
    [Documentation("MarkNotificationAsRead", "Marks the respective notification as read.")]
    public ResponseDto<bool> MarkNotificationAsRead([FromRoute] Guid notificationId)
    {
        notificationService.MarkAsRead(notificationId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Notification marked as read.",
            true);
    }

    [HttpPatch("read-all")]
    [Documentation("MarkAllNotificationsAsRead", "Marks all notifications for the logged in user as read.")]
    public ResponseDto<bool> MarkAllNotificationsAsRead()
    {
        notificationService.MarkAllAsRead();

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "All notifications marked as read.",
            true);
    }
}
