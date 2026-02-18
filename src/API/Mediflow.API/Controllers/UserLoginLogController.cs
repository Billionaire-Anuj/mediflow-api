using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.Domain.Common.Enum;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Users.LoginLog;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class UserLoginLogController(IUserLoginLogService userLoginLogService) : BaseController<UserLoginLogController>
{
    [HttpGet]
    [Documentation("GetAllUserLoginLogs", "Retrieve all paginated user login logs in the system.")]
    public CollectionDto<UserLoginLogDto> GetAllUserLoginLogs(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] List<Guid>? userIds = null,
        [FromQuery] string? emailAddressOrUsername = null,
        [FromQuery] List<LoginEventType>? eventTypes = null,
        [FromQuery] List<LoginStatus>? statuses = null,
        [FromQuery] string? ipAddress = null,
        [FromQuery] string? userAgent = null,
        [FromQuery] List<bool>? isActiveSession = null,
        [FromQuery] DateOnly? minimumActionDate = null,
        [FromQuery] DateOnly? maximumActionDate = null,
        [FromQuery] DateOnly? minimumLoggedOutDate = null,
        [FromQuery] DateOnly? maximumLoggedOutDate = null)
    {
        var result = userLoginLogService.GetAllUserLoginLogs(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            userIds,
            emailAddressOrUsername,
            eventTypes,
            statuses,
            ipAddress,
            userAgent,
            isActiveSession,
            minimumActionDate,
            maximumActionDate,
            minimumLoggedOutDate,
            maximumLoggedOutDate);

        return new CollectionDto<UserLoginLogDto>(
            (int)HttpStatusCode.OK,
            "The user login logs have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllUserLoginLogsList", "Retrieve all paginated user login logs in the system.")]
    public ResponseDto<List<UserLoginLogDto>> GetAllUserLoginLogs(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] List<Guid>? userIds = null,
        [FromQuery] string? emailAddressOrUsername = null,
        [FromQuery] List<LoginEventType>? eventTypes = null,
        [FromQuery] List<LoginStatus>? statuses = null,
        [FromQuery] string? ipAddress = null,
        [FromQuery] string? userAgent = null,
        [FromQuery] List<bool>? isActiveSession = null,
        [FromQuery] DateOnly? minimumActionDate = null,
        [FromQuery] DateOnly? maximumActionDate = null,
        [FromQuery] DateOnly? minimumLoggedOutDate = null,
        [FromQuery] DateOnly? maximumLoggedOutDate = null)
    {
        var result = userLoginLogService.GetAllUserLoginLogs(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            userIds,
            emailAddressOrUsername,
            eventTypes,
            statuses,
            ipAddress,
            userAgent,
            isActiveSession,
            minimumActionDate,
            maximumActionDate,
            minimumLoggedOutDate,
            maximumLoggedOutDate);

        return new ResponseDto<List<UserLoginLogDto>>(
            (int)HttpStatusCode.OK,
            "The user login logs have been successfully retrieved.",
            result);
    }

    [HttpGet("user/{userId:guid}")]
    [Documentation("GetAllUserLoginLogsByUserId", "Retrieve all paginated user login logs in the system via user identifier query.")]
    public CollectionDto<UserLoginLogDto> GetAllUserLoginLogsByUserId(
        [FromRoute] Guid userId,
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? emailAddressOrUsername = null,
        [FromQuery] List<LoginEventType>? eventTypes = null,
        [FromQuery] List<LoginStatus>? statuses = null,
        [FromQuery] string? ipAddress = null,
        [FromQuery] string? userAgent = null,
        [FromQuery] List<bool>? isActiveSession = null,
        [FromQuery] DateOnly? minimumActionDate = null,
        [FromQuery] DateOnly? maximumActionDate = null,
        [FromQuery] DateOnly? minimumLoggedOutDate = null,
        [FromQuery] DateOnly? maximumLoggedOutDate = null)
    {
        var result = userLoginLogService.GetAllUserLoginLogsByUserId(
            userId,
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            emailAddressOrUsername,
            eventTypes,
            statuses,
            ipAddress,
            userAgent,
            isActiveSession,
            minimumActionDate,
            maximumActionDate,
            minimumLoggedOutDate,
            maximumLoggedOutDate);

        return new CollectionDto<UserLoginLogDto>(
            (int)HttpStatusCode.OK,
            "The user login logs have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list/user/{userId:guid}")]
    [Documentation("GetAllUserLoginLogsByUserIdList", "Retrieve all non-paginated user login logs in the system via user identifier query.")]
    public ResponseDto<List<UserLoginLogDto>> GetAllUserLoginLogsByUserId(
        [FromRoute] Guid userId,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? emailAddressOrUsername = null,
        [FromQuery] List<LoginEventType>? eventTypes = null,
        [FromQuery] List<LoginStatus>? statuses = null,
        [FromQuery] string? ipAddress = null,
        [FromQuery] string? userAgent = null,
        [FromQuery] List<bool>? isActiveSession = null,
        [FromQuery] DateOnly? minimumActionDate = null,
        [FromQuery] DateOnly? maximumActionDate = null,
        [FromQuery] DateOnly? minimumLoggedOutDate = null,
        [FromQuery] DateOnly? maximumLoggedOutDate = null)
    {
        var result = userLoginLogService.GetAllUserLoginLogsByUserId(
            userId,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            emailAddressOrUsername,
            eventTypes,
            statuses,
            ipAddress,
            userAgent,
            isActiveSession,
            minimumActionDate,
            maximumActionDate,
            minimumLoggedOutDate,
            maximumLoggedOutDate);

        return new ResponseDto<List<UserLoginLogDto>>(
            (int)HttpStatusCode.OK,
            "The user login logs have been successfully retrieved.",
            result);
    }

    [HttpGet("{userLoginLogId:guid}")]
    [Documentation("GetUserLoginLogById", "Retrieve a record user login logs in the system via the identifier.")]
    public ResponseDto<UserLoginLogDto> GetUserLoginLogById([FromRoute] Guid userLoginLogId)
    {
        var result = userLoginLogService.GetUserLoginLogById(userLoginLogId);

        return new ResponseDto<UserLoginLogDto>(
            (int)HttpStatusCode.OK,
            "The user login log has been successfully retrieved.",
            result);
    }
}