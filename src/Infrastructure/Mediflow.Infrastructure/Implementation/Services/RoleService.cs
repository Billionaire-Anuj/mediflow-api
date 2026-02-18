using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class RoleService(IApplicationDbContext applicationDbContext) : IRoleService
{
    public List<RoleDto> GetAllRoles(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? description = null)
    {
        var roleModels = applicationDbContext.Roles
            .Where(x => 
                x.IsDisplayed &&
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Name.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (name == null || x.Name.ToLower().Contains(name.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys)
            .AsNoTracking()
            .AsQueryable();

        rowCount = roleModels.Count();

        return roleModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToRoleDto())
            .ToList();
    }

    public List<RoleDto> GetAllRoles(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? description = null)
    {
        var roleModels = applicationDbContext.Roles
            .Where(x => 
                x.IsDisplayed &&
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Name.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (name == null || x.Name.ToLower().Contains(name.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys)
            .AsNoTracking()
            .AsQueryable();

        return roleModels.Select(x => x.ToRoleDto()).ToList();
    }

    public List<RoleDto> GetAllAvailableRoles(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? description = null)
    {
        var roleModels = applicationDbContext.Roles
            .Where(x => 
                x.IsDisplayed && x.IsRegisterable &&
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Name.Contains(globalSearch.ToLower())
                    || x.Description.Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (name == null || x.Name.ToLower().Contains(name.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys)
            .AsNoTracking()
            .AsQueryable();
        
        rowCount = roleModels.Count();

        return roleModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToRoleDto())
            .ToList();
    }

    public List<RoleDto> GetAllAvailableRoles(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? description = null)
    {
        var roleModels = applicationDbContext.Roles
            .Where(x => 
                x.IsDisplayed && x.IsRegisterable &&
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Name.Contains(globalSearch.ToLower())
                    || x.Description.Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (name == null || x.Name.ToLower().Contains(name.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys)
            .AsNoTracking()
            .AsQueryable();

        return roleModels.Select(x => x.ToRoleDto()).ToList();
    }

    public RoleDto GetRoleById(Guid roleId)
    {
        var role = applicationDbContext.Roles
                       .AsNoTracking()
                       .FirstOrDefault(x => x.Id == roleId)
                   ?? throw new NotFoundException("The respective role could not be found.");

        return role.ToRoleDto();
    }

    public void CreateRole(CreateRoleDto role)
    {
        var duplicateRole = applicationDbContext.Roles.FirstOrDefault(x => x.Name == role.Name);

        if (duplicateRole != null)
        {
            throw new BadRequestException($"A role with name '{role.Name}' already exists.");
        }

        var roleModel = new Role(role.Name, role.Description, true, true);

        applicationDbContext.Roles.Add(roleModel);

        applicationDbContext.SaveChanges();
    }

    public void UpdateRole(Guid roleId, UpdateRoleDto role)
    {
        if (roleId != role.Id)
        {
            throw new BadRequestException("Route identifier does not match payload identifier.");
        }

        var roleModel = applicationDbContext.Roles.FirstOrDefault(x => x.Id == roleId)
                        ?? throw new NotFoundException($"Role with identifier '{roleId}' was not found.");

        if (!roleModel.IsRegisterable)
        {
            throw new BadRequestException("The following role cannot be modified.");
        }

        var duplicateRole = applicationDbContext.Roles.FirstOrDefault(x => x.Name == role.Name);

        if (duplicateRole != null)
        {
            throw new BadRequestException($"A role with name '{role.Name}' already exists.");
        }

        roleModel.Update(role.Name, role.Description, roleModel.IsDisplayed, roleModel.IsRegisterable);

        applicationDbContext.Roles.Update(roleModel);

        applicationDbContext.SaveChanges();
    }

    public void ActivateDeactivateRole(Guid roleId)
    {
        var roleModel = applicationDbContext.Roles.FirstOrDefault(x => x.Id == roleId)
                        ?? throw new NotFoundException($"Role with identifier '{roleId}' was not found.");

        if (!roleModel.IsRegisterable)
        {
            throw new BadRequestException("The following role cannot be modified.");
        }

        roleModel.ActivateDeactivateEntity();

        applicationDbContext.Roles.Update(roleModel);

        applicationDbContext.SaveChanges();
    }
}
