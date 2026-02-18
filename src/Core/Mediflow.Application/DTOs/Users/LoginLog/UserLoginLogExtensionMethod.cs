using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Users.LoginLog;

public static class UserLoginLogExtensionMethod
{
    public static UserLoginLogDto ToUserLoginLogDto(this UserLoginLog userLoginLog)
    {
        return new UserLoginLogDto
        {
            User = userLoginLog.User?.ToUserDto(),
            EmailAddressOrUsername = userLoginLog.EmailAddressOrUsername,
            EventType = userLoginLog.EventType,
            Status = userLoginLog.Status,
            AccessToken = userLoginLog.AccessToken,
            IpAddress = userLoginLog.IpAddress,
            UserAgent = userLoginLog.UserAgent,
            IsActiveSession = userLoginLog.IsActiveSession,
            ActionDate = userLoginLog.ActionDate,
            LoggedOutDate = userLoginLog.LoggedOutDate
        };
    }
}