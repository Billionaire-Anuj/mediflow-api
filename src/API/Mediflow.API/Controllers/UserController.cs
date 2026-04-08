using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.DTOs.Users;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Mediflow.Domain.Common;

namespace Mediflow.API.Controllers;

public class UserController(IUserService userService) : BaseController<UserController>
{
    [HttpGet]
    [Documentation("GetAllUsers", "Retrieve all available users in the system.")]
    public CollectionDto<UserDto> GetAllUsers(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? name = null,
        [FromQuery] string? username = null,
        [FromQuery] string? emailAddress = null,
        [FromQuery] string? address = null,
        [FromQuery] string? phoneNumber = null,
        [FromQuery] List<Guid>? roleIds = null)
    {
        var result = userService.GetAllUsers(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            name,
            username,
            emailAddress,
            address,
            phoneNumber,
            roleIds);

        return new CollectionDto<UserDto>(
            (int)HttpStatusCode.OK,
            "The users have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Attributes.Authorize(Constants.Roles.SuperAdmin.Name, Constants.Roles.TenantAdministrator.Name)]
    [Documentation("GetAllUsersList", "Retrieve all available users in the system.")]
    public ResponseDto<List<UserDto>> GetAllUsers(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? name = null,
        [FromQuery] string? username = null,
        [FromQuery] string? emailAddress = null,
        [FromQuery] string? address = null,
        [FromQuery] string? phoneNumber = null,
        [FromQuery] List<Guid>? roleIds = null)
    {
        var result = userService.GetAllUsers(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            name,
            username,
            emailAddress,
            address,
            phoneNumber,
            roleIds);

        return new ResponseDto<List<UserDto>>(
            (int)HttpStatusCode.OK,
            "The users have been successfully retrieved.",
            result);
    }

    [HttpGet("{userId:guid}")]
    [Documentation("GetUserById", "Retrieve the respective user via its identifier in the system.")]
    public ResponseDto<UserDto> GetUserById([FromRoute] Guid userId)
    {
        var result = userService.GetUserById(userId);

        return new ResponseDto<UserDto>(
            (int)HttpStatusCode.OK,
            "User successfully fetched.",
            result);
    }

    [HttpPost]
    [AllowAnonymous]
    [Documentation("RegisterUser", "Registers a new record of user.")]
    public ResponseDto<bool> RegisterUser([FromForm] RegisterUserDto user)
    {
        userService.RegisterUser(user);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "User successfully created.",
            true);
    }

    [HttpPost("admin/register")]
    [Attributes.Authorize(Constants.Roles.SuperAdmin.Name, Constants.Roles.TenantAdministrator.Name)]
    [Documentation("RegisterUserByAdmin", "Registers a new user via admin and sends credentials by email.")]
    public ResponseDto<bool> RegisterUserByAdmin([FromForm] RegisterUserByAdminDto user)
    {
        userService.RegisterUserByAdmin(user);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "User successfully registered. Credentials have been emailed.",
            true);
    }

    [HttpPut("{userId:guid}")]
    [Documentation("UpdateUser", "Updates an existing record of user.")]
    public ResponseDto<bool> UpdateUser([FromRoute] Guid userId, [FromForm] UpdateUserDto user)
    {
        userService.UpdateUser(userId, user);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "User successfully updated.",
            true);
    }

    [HttpPatch("{userId:guid}/reset/password")]
    [Documentation("ResetPassword", "Resets a password for a respective user generating a random password.")]
    public ResponseDto<bool> ResetPassword([FromRoute] Guid userId)
    {
        userService.ResetPassword(userId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Password changed successfully.",
            true);
    }

    [HttpPatch("{userId:guid}/status")]
    [Documentation("ActivateDeactivateUser", "Updates the activation status of a user.")]
    public ResponseDto<bool> ActivateDeactivateUser([FromRoute] Guid userId)
    {
        userService.ActivateDeactivateUser(userId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "User status successfully updated.",
            true);
    }
}
