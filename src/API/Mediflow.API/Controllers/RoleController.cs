using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Mediflow.API.Controllers;

public class RoleController(IRoleService roleService) : BaseController<RoleController>
{
    [HttpGet]
    [Documentation("GetAllRoles", "Retrieve all paginated roles in the system.")]
    public CollectionDto<RoleDto> GetAllRoles(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? name = null,
        [FromQuery] string? description = null)
    {
        var result = roleService.GetAllRoles(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            name,
            description);

        return new CollectionDto<RoleDto>(
            (int)HttpStatusCode.OK,
            "The roles have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllRolesList", "Retrieve all non-paginated roles in the system.")]
    public ResponseDto<List<RoleDto>> GetAllRoles(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? name = null,
        [FromQuery] string? description = null)
    {
        var result = roleService.GetAllRoles(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            name,
            description);

        return new ResponseDto<List<RoleDto>>(
            (int)HttpStatusCode.OK,
            "The roles have been successfully retrieved.",
            result);
    }

    [HttpGet("available")]
    [Documentation("GetAllAvailableRoles", "Retrieve all paginated available roles in the system.")]
    public CollectionDto<RoleDto> GetAllAvailableRoles(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? name = null,
        [FromQuery] string? description = null)
    {
        var result = roleService.GetAllAvailableRoles(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            name,
            description);

        return new CollectionDto<RoleDto>(
            (int)HttpStatusCode.OK,
            "The roles have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [AllowAnonymous]
    [HttpGet("list/available")]
    [Documentation("GetAllAvailableRolesList", "Retrieve all non-paginated available roles in the system.")]
    public ResponseDto<List<RoleDto>> GetAllAvailableRoles(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? name = null,
        [FromQuery] string? description = null)
    {
        var result = roleService.GetAllAvailableRoles(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            name,
            description);

        return new ResponseDto<List<RoleDto>>(
            (int)HttpStatusCode.OK,
            "The roles have been successfully retrieved.",
            result);
    }

    [HttpGet("{roleId:guid}")]
    [Documentation("GetRoleById", "Retrieve the respective role via its identifier in the system.")]
    public ResponseDto<RoleDto> GetRoleById([FromRoute] Guid roleId)
    {
        var result = roleService.GetRoleById(roleId);

        return new ResponseDto<RoleDto>(
            (int)HttpStatusCode.OK,
            "Role successfully fetched.",
            result);
    }

    [HttpPost]
    [Documentation("CreateRole", "Creates a new record of role.")]
    public ResponseDto<bool> CreateRole([FromBody] CreateRoleDto role)
    {
        roleService.CreateRole(role);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Role successfully created.",
            true);
    }

    [HttpPut("{roleId:guid}")]
    [Documentation("UpdateRole", "Updates an existing record of role.")]
    public ResponseDto<bool> UpdateRole([FromRoute] Guid roleId, [FromBody] UpdateRoleDto role)
    {
        roleService.UpdateRole(roleId, role);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Role successfully updated.",
            true);
    }

    [HttpPatch("{roleId:guid}/status")]
    [Documentation("ActivateDeactivateRole", "Updates the activation status of a role.")]
    public ResponseDto<bool> ActivateDeactivateRole([FromRoute] Guid roleId)
    {
        roleService.ActivateDeactivateRole(roleId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Role status successfully updated.",
            true);
    }
}