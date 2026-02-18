using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Users.LoginLog;

namespace Mediflow.Application.Interfaces.Services;

public interface IUserLoginLogService : ITransientService
{
    List<UserLoginLogDto> GetAllUserLoginLogs(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? userIds = null,
        string? emailAddressOrUsername = null,
        List<LoginEventType>? eventTypes = null,
        List<LoginStatus>? statuses = null,
        string? ipAddress = null,
        string? userAgent = null,
        List<bool>? isActiveSession = null,
        DateOnly? minimumActionDate = null,
        DateOnly? maximumActionDate = null,
        DateOnly? minimumLoggedOutDate = null,
        DateOnly? maximumLoggedOutDate = null);

    List<UserLoginLogDto> GetAllUserLoginLogs(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? userIds = null,
        string? emailAddressOrUsername = null,
        List<LoginEventType>? eventTypes = null,
        List<LoginStatus>? statuses = null,
        string? ipAddress = null,
        string? userAgent = null,
        List<bool>? isActiveSession = null,
        DateOnly? minimumActionDate = null,
        DateOnly? maximumActionDate = null,
        DateOnly? minimumLoggedOutDate = null,
        DateOnly? maximumLoggedOutDate = null);

    List<UserLoginLogDto> GetAllUserLoginLogsByUserId(
        Guid userId,
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? emailAddressOrUsername = null,
        List<LoginEventType>? eventTypes = null,
        List<LoginStatus>? statuses = null,
        string? ipAddress = null,
        string? userAgent = null,
        List<bool>? isActiveSession = null,
        DateOnly? minimumActionDate = null,
        DateOnly? maximumActionDate = null,
        DateOnly? minimumLoggedOutDate = null,
        DateOnly? maximumLoggedOutDate = null);

    List<UserLoginLogDto> GetAllUserLoginLogsByUserId(
        Guid userId,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? emailAddressOrUsername = null,
        List<LoginEventType>? eventTypes = null,
        List<LoginStatus>? statuses = null,
        string? ipAddress = null,
        string? userAgent = null,
        List<bool>? isActiveSession = null,
        DateOnly? minimumActionDate = null,
        DateOnly? maximumActionDate = null,
        DateOnly? minimumLoggedOutDate = null,
        DateOnly? maximumLoggedOutDate = null);

    UserLoginLogDto GetUserLoginLogById(Guid userLoginLogId);
}