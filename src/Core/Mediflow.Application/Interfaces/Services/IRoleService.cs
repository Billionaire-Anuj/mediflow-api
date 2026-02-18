using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface IRoleService : ITransientService
{
    List<RoleDto> GetAllRoles(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? description = null
    );

    List<RoleDto> GetAllRoles(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? description = null
    );

    // Available Roles define the roles that can be assigned to users via the IsRegisterable flag.
    List<RoleDto> GetAllAvailableRoles(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? description = null
    );

    // Available Roles define the roles that can be assigned to users via the IsRegisterable flag.
    List<RoleDto> GetAllAvailableRoles(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? description = null
    );

    RoleDto GetRoleById(Guid roleId);

    void CreateRole(CreateRoleDto role);

    void UpdateRole(Guid roleId, UpdateRoleDto role);

    void ActivateDeactivateRole(Guid roleId);
}