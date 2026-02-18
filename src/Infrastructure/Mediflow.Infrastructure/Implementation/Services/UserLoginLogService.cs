using Mediflow.Domain.Common.Enum;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.DTOs.Users.LoginLog;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class UserLoginLogService(IApplicationDbContext applicationDbContext) : IUserLoginLogService
{
    public List<UserLoginLogDto> GetAllUserLoginLogs(
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
        DateOnly? maximumLoggedOutDate = null)
    {
        var userIdentifiers = userIds != null ? new HashSet<Guid>(userIds) : null;
        var eventTypeIdentifiers = eventTypes != null ? new HashSet<LoginEventType>(eventTypes) : null;
        var statusIdentifiers = statuses != null ? new HashSet<LoginStatus>(statuses) : null;
        var isActiveSessionIdentifiers = isActiveSession != null ? new HashSet<bool>(isActiveSession) : null;

        var userLoginLogModels = applicationDbContext.UserLoginLogs
            .Where(x => 
                (string.IsNullOrEmpty(globalSearch)
                    || x.EmailAddressOrUsername.ToLower().Contains(globalSearch.ToLower())
                    || x.EventType.ToString().ToLower().Contains(globalSearch.ToLower())
                    || x.Status.ToString().ToLower().Contains(globalSearch.ToLower())
                    || (x.IpAddress != null && x.IpAddress.ToString().Contains(globalSearch.ToLower()))
                    || (x.UserAgent != null && x.UserAgent.ToString().Contains(globalSearch.ToLower()))
                    || x.ActionDate.ToString("dd-MM-yyyy").Contains(globalSearch.ToLower())
                    || (x.LoggedOutDate != null && x.LoggedOutDate.Value.ToString("dd-MM-yyyy").Contains(globalSearch.ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (userIdentifiers == null || (x.UserId.HasValue && userIdentifiers.Contains(x.UserId.Value))) &&
                (string.IsNullOrEmpty(emailAddressOrUsername) || x.EmailAddressOrUsername.ToLower().Contains(emailAddressOrUsername.ToLower())) &&
                (eventTypeIdentifiers == null || eventTypeIdentifiers.Contains(x.EventType)) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (string.IsNullOrEmpty(ipAddress) || (x.IpAddress != null && x.IpAddress.ToLower().Contains(ipAddress.ToLower()))) &&
                (string.IsNullOrEmpty(userAgent) || (x.UserAgent != null && x.UserAgent.ToLower().Contains(userAgent.ToLower()))) &&
                (isActiveSessionIdentifiers == null || isActiveSessionIdentifiers.Contains(x.IsActiveSession)) &&
                (minimumActionDate == null || DateOnly.FromDateTime(x.ActionDate) >= minimumActionDate) &&
                (maximumActionDate == null || DateOnly.FromDateTime(x.ActionDate) <= maximumActionDate) &&
                (minimumLoggedOutDate == null || (x.LoggedOutDate != null && DateOnly.FromDateTime(x.LoggedOutDate.Value) >= minimumLoggedOutDate)) &&
                (maximumLoggedOutDate == null || (x.LoggedOutDate != null && DateOnly.FromDateTime(x.LoggedOutDate.Value) <= minimumLoggedOutDate)))
            .OrderBy(x => x.Id)
            .Include(x => x.User)
                .ThenInclude(x => x!.Role)
            .AsNoTracking()
            .AsQueryable();

        rowCount = userLoginLogModels.Count();

        return userLoginLogModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToUserLoginLogDto())
            .ToList();
    }

    public List<UserLoginLogDto> GetAllUserLoginLogs(
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
        DateOnly? maximumLoggedOutDate = null)
    {
        var userIdentifiers = userIds != null ? new HashSet<Guid>(userIds) : null;
        var eventTypeIdentifiers = eventTypes != null ? new HashSet<LoginEventType>(eventTypes) : null;
        var statusIdentifiers = statuses != null ? new HashSet<LoginStatus>(statuses) : null;
        var isActiveSessionIdentifiers = isActiveSession != null ? new HashSet<bool>(isActiveSession) : null;

        var userLoginLogModels = applicationDbContext.UserLoginLogs
            .Where(x => 
                (string.IsNullOrEmpty(globalSearch)
                    || x.EmailAddressOrUsername.ToLower().Contains(globalSearch.ToLower())
                    || x.EventType.ToString().ToLower().Contains(globalSearch.ToLower())
                    || x.Status.ToString().ToLower().Contains(globalSearch.ToLower())
                    || (x.IpAddress != null && x.IpAddress.ToString().Contains(globalSearch.ToLower()))
                    || (x.UserAgent != null && x.UserAgent.ToString().Contains(globalSearch.ToLower()))
                    || x.ActionDate.ToString("dd-MM-yyyy").Contains(globalSearch.ToLower())
                    || (x.LoggedOutDate != null && x.LoggedOutDate.Value.ToString("dd-MM-yyyy").Contains(globalSearch.ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (userIdentifiers == null || (x.UserId.HasValue && userIdentifiers.Contains(x.UserId.Value))) &&
                (string.IsNullOrEmpty(emailAddressOrUsername) || x.EmailAddressOrUsername.ToLower().Contains(emailAddressOrUsername.ToLower())) &&
                (eventTypeIdentifiers == null || eventTypeIdentifiers.Contains(x.EventType)) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (string.IsNullOrEmpty(ipAddress) || (x.IpAddress != null && x.IpAddress.ToLower().Contains(ipAddress.ToLower()))) &&
                (string.IsNullOrEmpty(userAgent) || (x.UserAgent != null && x.UserAgent.ToLower().Contains(userAgent.ToLower()))) &&
                (isActiveSessionIdentifiers == null || isActiveSessionIdentifiers.Contains(x.IsActiveSession)) &&
                (minimumActionDate == null || DateOnly.FromDateTime(x.ActionDate) >= minimumActionDate) &&
                (maximumActionDate == null || DateOnly.FromDateTime(x.ActionDate) <= maximumActionDate) &&
                (minimumLoggedOutDate == null || (x.LoggedOutDate != null && DateOnly.FromDateTime(x.LoggedOutDate.Value) >= minimumLoggedOutDate)) &&
                (maximumLoggedOutDate == null || (x.LoggedOutDate != null && DateOnly.FromDateTime(x.LoggedOutDate.Value) <= minimumLoggedOutDate)))
            .OrderBy(x => x.Id)
            .Include(x => x.User)
                .ThenInclude(x => x!.Role)
            .AsNoTracking()
            .AsQueryable();

        return userLoginLogModels.Select(x => x.ToUserLoginLogDto()).ToList();
    }

    public List<UserLoginLogDto> GetAllUserLoginLogsByUserId(
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
        DateOnly? maximumLoggedOutDate = null)
    {
        var user = applicationDbContext.Users
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"User with identifier '{userId}' was not found.");

        var eventTypeIdentifiers = eventTypes != null ? new HashSet<LoginEventType>(eventTypes) : null;
        var statusIdentifiers = statuses != null ? new HashSet<LoginStatus>(statuses) : null;
        var isActiveSessionIdentifiers = isActiveSession != null ? new HashSet<bool>(isActiveSession) : null;

        var userLoginLogModels = applicationDbContext.UserLoginLogs
            .Where(x => 
                x.UserId == user.Id &&
                (string.IsNullOrEmpty(globalSearch)
                    || x.EmailAddressOrUsername.ToLower().Contains(globalSearch.ToLower())
                    || x.EventType.ToString().ToLower().Contains(globalSearch.ToLower())
                    || x.Status.ToString().ToLower().Contains(globalSearch.ToLower())
                    || (x.IpAddress != null && x.IpAddress.ToString().Contains(globalSearch.ToLower()))
                    || (x.UserAgent != null && x.UserAgent.ToString().Contains(globalSearch.ToLower()))
                    || x.ActionDate.ToString("dd-MM-yyyy").Contains(globalSearch.ToLower())
                    || (x.LoggedOutDate != null && x.LoggedOutDate.Value.ToString("dd-MM-yyyy").Contains(globalSearch.ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (string.IsNullOrEmpty(emailAddressOrUsername) || x.EmailAddressOrUsername.ToLower().Contains(emailAddressOrUsername.ToLower())) &&
                (eventTypeIdentifiers == null || eventTypeIdentifiers.Contains(x.EventType)) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (string.IsNullOrEmpty(ipAddress) || (x.IpAddress != null && x.IpAddress.ToLower().Contains(ipAddress.ToLower()))) &&
                (string.IsNullOrEmpty(userAgent) || (x.UserAgent != null && x.UserAgent.ToLower().Contains(userAgent.ToLower()))) &&
                (isActiveSessionIdentifiers == null || isActiveSessionIdentifiers.Contains(x.IsActiveSession)) &&
                (minimumActionDate == null || DateOnly.FromDateTime(x.ActionDate) >= minimumActionDate) &&
                (maximumActionDate == null || DateOnly.FromDateTime(x.ActionDate) <= maximumActionDate) &&
                (minimumLoggedOutDate == null || (x.LoggedOutDate != null && DateOnly.FromDateTime(x.LoggedOutDate.Value) >= minimumLoggedOutDate)) &&
                (maximumLoggedOutDate == null || (x.LoggedOutDate != null && DateOnly.FromDateTime(x.LoggedOutDate.Value) <= minimumLoggedOutDate)))
            .OrderBy(x => x.Id)
            .Include(x => x.User)
                .ThenInclude(x => x!.Role)
            .AsNoTracking()
            .AsQueryable();

        rowCount = userLoginLogModels.Count();

        return userLoginLogModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToUserLoginLogDto())
            .ToList();
    }

    public List<UserLoginLogDto> GetAllUserLoginLogsByUserId(
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
        DateOnly? maximumLoggedOutDate = null)
    {
        var user = applicationDbContext.Users
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"User with identifier '{userId}' was not found.");

        var eventTypeIdentifiers = eventTypes != null ? new HashSet<LoginEventType>(eventTypes) : null;
        var statusIdentifiers = statuses != null ? new HashSet<LoginStatus>(statuses) : null;
        var isActiveSessionIdentifiers = isActiveSession != null ? new HashSet<bool>(isActiveSession) : null;

        var userLoginLogModels = applicationDbContext.UserLoginLogs
            .Where(x => 
                x.UserId == user.Id &&
                (string.IsNullOrEmpty(globalSearch)
                    || x.EmailAddressOrUsername.ToLower().Contains(globalSearch.ToLower())
                    || x.EventType.ToString().ToLower().Contains(globalSearch.ToLower())
                    || x.Status.ToString().ToLower().Contains(globalSearch.ToLower())
                    || (x.IpAddress != null && x.IpAddress.ToString().Contains(globalSearch.ToLower()))
                    || (x.UserAgent != null && x.UserAgent.ToString().Contains(globalSearch.ToLower()))
                    || x.ActionDate.ToString("dd-MM-yyyy").Contains(globalSearch.ToLower())
                    || (x.LoggedOutDate != null && x.LoggedOutDate.Value.ToString("dd-MM-yyyy").Contains(globalSearch.ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (string.IsNullOrEmpty(emailAddressOrUsername) || x.EmailAddressOrUsername.ToLower().Contains(emailAddressOrUsername.ToLower())) &&
                (eventTypeIdentifiers == null || eventTypeIdentifiers.Contains(x.EventType)) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (string.IsNullOrEmpty(ipAddress) || (x.IpAddress != null && x.IpAddress.ToLower().Contains(ipAddress.ToLower()))) &&
                (string.IsNullOrEmpty(userAgent) || (x.UserAgent != null && x.UserAgent.ToLower().Contains(userAgent.ToLower()))) &&
                (isActiveSessionIdentifiers == null || isActiveSessionIdentifiers.Contains(x.IsActiveSession)) &&
                (minimumActionDate == null || DateOnly.FromDateTime(x.ActionDate) >= minimumActionDate) &&
                (maximumActionDate == null || DateOnly.FromDateTime(x.ActionDate) <= maximumActionDate) &&
                (minimumLoggedOutDate == null || (x.LoggedOutDate != null && DateOnly.FromDateTime(x.LoggedOutDate.Value) >= minimumLoggedOutDate)) &&
                (maximumLoggedOutDate == null || (x.LoggedOutDate != null && DateOnly.FromDateTime(x.LoggedOutDate.Value) <= minimumLoggedOutDate)))
            .OrderBy(x => x.Id)
            .Include(x => x.User)
                .ThenInclude(x => x!.Role)
            .AsNoTracking()
            .AsQueryable();

        return userLoginLogModels.Select(x => x.ToUserLoginLogDto()).ToList();
    }

    public UserLoginLogDto GetUserLoginLogById(Guid userLoginLogId)
    {
        var userLoginLogModel = applicationDbContext.UserLoginLogs
                                    .Include(x => x.User)
                                        .ThenInclude(x => x!.Role)
                                    .AsNoTracking()
                                    .FirstOrDefault(x => x.Id == userLoginLogId)
                                ?? throw new NotFoundException($"User login log with identifier '{userLoginLogId}' was not found.");

        return userLoginLogModel.ToUserLoginLogDto();
    }
}
