using Mediflow.Helper;
using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Settings;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class AdministratorsSeeder(
    IOptions<SeedSettings> seedOptions,
    ILogger<AdministratorsSeeder> logger,
    IApplicationDbContext applicationDbContext) : IDataSeeder
{
    private readonly SeedSettings _seedSettings = seedOptions.Value;

    public int Order => 40;

    public void Seed()
    {
        var superAdminRole = applicationDbContext.Roles
                                 .FirstOrDefault(x => x.Id == Guid.Parse(Constants.Roles.SuperAdmin.Id))
                             ?? throw new NotFoundException("Super admin role could not be found.");

        var tenantAdministratorRole = applicationDbContext.Roles
                                      .FirstOrDefault(x => x.Id == Guid.Parse(Constants.Roles.TenantAdministrator.Id))
                                      ?? throw new NotFoundException("Tenant administrator role could not be found.");

        InitializeAdministrator(_seedSettings.SuperAdmin, superAdminRole);
        InitializeAdministrator(_seedSettings.TenantAdministrator, tenantAdministratorRole);

        logger.LogInformation("Administrators initialization successfully completed.");
    }

    private void InitializeAdministrator(UserSeedSettings userSeedSettings, Role role)
    {
        var userIdentifier = new Guid(userSeedSettings.Identifier);

        var user = applicationDbContext.Users.FirstOrDefault(x => x.Id == userIdentifier);

        if (user is null)
        {
            var userModel = new User(
                role.Id,
                Gender.Male,
                userSeedSettings.Name,
                userSeedSettings.Username,
                userSeedSettings.EmailAddress,
                userSeedSettings.Address,
                null,
                userSeedSettings.Password.Hash(),
                userSeedSettings.PhoneNumber);

            userModel.AssignIdentifier(userIdentifier);

            applicationDbContext.Users.Add(userModel);

            logger.LogInformation($"Administrator with identifier {userModel.Id} successfully registered.");
        }
        else
        {
            if (role is { IsDisplayed: true, IsRegisterable: false })
            {
                user.Update(
                    role.Id,
                    Gender.Male,
                    userSeedSettings.Name,
                    userSeedSettings.Username,
                    userSeedSettings.EmailAddress,
                    userSeedSettings.Address,
                    userSeedSettings.PhoneNumber);

                applicationDbContext.Users.Update(user);

                logger.LogInformation($"Administrator with identifier {user.Id} successfully updated.");
            }
        }

        applicationDbContext.SaveChanges();
    }
}
