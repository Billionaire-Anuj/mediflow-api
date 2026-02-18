using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.DTOs.Permissions;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.Common.Authorization;

namespace Mediflow.Infrastructure.Implementation.Services;

public class PermissionService(IApplicationDbContext applicationDbContext, IApplicationUserService userService) : IPermissionService
{
    private const string TenantAdministratorIdentifier = Constants.Roles.TenantAdministrator.Id;

    private static readonly string[] Actions =
    [
        MediflowActions.Menu,
        MediflowActions.View,
        MediflowActions.Create,
        MediflowActions.Update,
        MediflowActions.Delete,
        MediflowActions.ActivateDeactivate
    ];

    public bool HasPermission(string action, string resource)
    {
        var userId = userService.GetUserId;
        
        if (userId == Guid.Empty) throw new UnauthorizedException("Authorization token is missing. Please try again by logging in.");
        
        var user = applicationDbContext.Users
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == userId)
                   ?? throw new NotFoundException("The following user was not found.");

        var role = applicationDbContext.Roles
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == user.RoleId)
                   ?? throw new NotFoundException("The following role could not be found.");

        // Tenant Administrator will have all permissions.
        if (role.Id == Guid.Parse(TenantAdministratorIdentifier)) return true;

        var resourceModel = applicationDbContext.Resources
                                .AsNoTracking()
                                .FirstOrDefault(x => x.Name == resource) 
                            ?? throw new NotFoundException("The respective resource could not be found.");
        
        var result = applicationDbContext.Permissions.FirstOrDefault(x => 
            x.ResourceId == resourceModel.Id && x.Action == action && x.RoleId == role.Id);
        
        return result != null;
    }

    public List<AssignedPermissionsDto> GetAssignedPermissions()
    {
        var userId = userService.GetUserId;
        
        if (userId == Guid.Empty) throw new UnauthorizedException("Authorization token is missing. Please try again by logging in.");
        
        var user = applicationDbContext.Users
                       .AsNoTracking()
                       .FirstOrDefault(x => x.Id == userId)
                   ?? throw new NotFoundException("The following user was not found.");

        var role = applicationDbContext.Roles
                       .AsNoTracking()
                       .FirstOrDefault(x => x.Id == user.RoleId)
                   ?? throw new NotFoundException("The following role could not be found.");
        
        return GetAllocatedPermissions(role.Id);
    }
    
    public List<AssignedPermissionsDto> GetAllocatedPermissions(Guid roleId)
    {
        var role = applicationDbContext.Roles
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == roleId)
                   ?? throw new NotFoundException("The following role could not be found.");
        
        var permissions = applicationDbContext.Permissions
            .AsNoTracking()
            .Where(x => x.RoleId == role.Id).AsQueryable();
        
        var resources = applicationDbContext.Resources
            .AsNoTracking()
            .AsQueryable(); 
        
        var assignedPermissions = new List<AssignedPermissionsDto>(); 

        foreach (var resource in resources)
        {
            var assignedPermission = new AssignedPermissionsDto()
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
            };
            
            var resourcePermissions = permissions.Where(x => x.ResourceId == resource.Id).ToList();
            
            foreach (var action in Actions)
            {
                var resourcePermission = resourcePermissions.FirstOrDefault(x => x.Action == action);
                
                var assignedAction = new PermissionDto()
                {
                    Action = action,
                    IsAssigned = resourcePermission != null
                };
                
                assignedPermission.Permissions.Add(assignedAction);
            }
            
            assignedPermissions.Add(assignedPermission);
        }

        return assignedPermissions;
    }

    public void GrantPermission(GrantPermissionsDto grantPermission)
    {
        var role = applicationDbContext.Roles
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == grantPermission.RoleId) 
                   ?? throw new NotFoundException("The role could not be found.");
        
        var permissions = applicationDbContext.Permissions.Where(x => x.RoleId == grantPermission.RoleId).AsQueryable();

        if (permissions.Any())
        {
            applicationDbContext.Permissions.RemoveRange(permissions);
        }
        
        var permissionModels = new List<Permission>();
        
        foreach (var permission in grantPermission.Permissions)
        {
            var resource = applicationDbContext.Resources
                           .AsNoTracking()
                           .FirstOrDefault(x => x.Id == permission.ResourceId) 
                           ?? throw new NotFoundException("The resource could not be found.");

            foreach (var action in permission.Actions)
            {
                if (action is MediflowActions.Menu or MediflowActions.View or MediflowActions.Create or MediflowActions.Update or MediflowActions.Delete or MediflowActions.ActivateDeactivate)
                {
                    permissionModels.Add(new Permission(role.Id, resource.Id, action));
                }
            }
        }

        applicationDbContext.Permissions.AddRange(permissionModels);

        applicationDbContext.SaveChanges();
    }
}
