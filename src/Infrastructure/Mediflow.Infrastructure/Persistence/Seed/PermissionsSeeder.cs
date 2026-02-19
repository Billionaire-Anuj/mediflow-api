using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;
using Mediflow.Application.Common.Authorization;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class PermissionsSeeder(ILogger<PermissionsSeeder> logger, IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public int Order => 30;

    public void Seed()
    {
        // We do not need to assign permissions to other roles as it will be manually adjusted via the SA.
        // We also do not need to seed permissions for the tenant administrator role as the permission flag check will bypass it.
        var superAdminRole = applicationDbContext.Roles
                                .FirstOrDefault(x => x.Id == Guid.Parse(Constants.Roles.SuperAdmin.Id)) 
                                ?? throw new NotFoundException("Super admin role could not be found.");

        var resources = applicationDbContext.Resources;

        if (resources.Any())
        {
            logger.LogWarning("No resources found. Please ensure resource seeding runs before permissions.");
        }

        var actions = new List<string>
        {
            MediflowActions.Menu,
            MediflowActions.View,
            MediflowActions.Create,
            MediflowActions.Update,
            MediflowActions.Delete,
            MediflowActions.ActivateDeactivate
        };

        foreach (var resource in resources)
        {
            foreach (var action in actions)
            {
                var permissionExists = applicationDbContext.Permissions.Any(x => 
                    x.RoleId == superAdminRole.Id && x.ResourceId == resource.Id && x.Action == action);

                if (permissionExists) continue;

                var permission = new Permission(superAdminRole.Id, resource.Id, action);

                applicationDbContext.Permissions.Add(permission);

                logger.LogInformation(
                    "Addition of '{Action}' permission for resource '{ResourceName}' (Super Admin).",
                    action,
                    resource.Name);
            }
        }

        applicationDbContext.SaveChanges();

        logger.LogInformation("Super admin permissions initialization successfully completed.");
    }
}
