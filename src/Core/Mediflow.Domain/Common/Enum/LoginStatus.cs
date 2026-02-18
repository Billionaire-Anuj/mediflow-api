namespace Mediflow.Domain.Common.Enum;

public enum LoginStatus
{
    Success,
    FailedUserNotFound,
    FailedInvalidPassword,
    Pending2FactorAuthentication,
    FailedInactiveUser,
    LoggedOut,
    ForcedLogout
}