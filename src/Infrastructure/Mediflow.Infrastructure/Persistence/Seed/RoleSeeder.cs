using System.Reflection;
using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class RoleSeeder(ILogger<RoleSeeder> logger, IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public int Order => 10;

    public void Seed()
    {
        var roleSpecifications = GetRoleSpecificationFromConstants();

        foreach (var roleSpecification in roleSpecifications)
        {
            var role = applicationDbContext.Roles.FirstOrDefault(x => x.Id == roleSpecification.Id);

            if (role != null)
            {
                role.Update(
                    roleSpecification.Name,
                    roleSpecification.Description,
                    roleSpecification.IsDisplayed,
                    roleSpecification.IsRegisterable);

                applicationDbContext.Roles.Update(role);
            }
            else
            {
                var roleModel = new Role(
                    roleSpecification.Name,
                    roleSpecification.Description,
                    roleSpecification.IsDisplayed,
                    roleSpecification.IsRegisterable);

                roleModel.AssignIdentifier(roleSpecification.Id);

                applicationDbContext.Roles.Add(roleModel);

                logger.LogInformation("Seeded role {RoleName} with the identifier {RoleId}.", roleSpecification.Name, roleSpecification.Id);
            }
        }

        applicationDbContext.SaveChanges();

        logger.LogInformation("Roles initialization successfully completed.");
    }

    private sealed record RoleSpecification(Guid Id, string Name, string Description, bool IsDisplayed, bool IsRegisterable);

    private static IEnumerable<RoleSpecification> GetRoleSpecificationFromConstants()
    {
        var roleSpecifications = new List<RoleSpecification>();

        foreach (var role in typeof(Constants.Roles).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
        {
            var idField = role.GetField("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var nameField = role.GetField("Name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var descriptionField = role.GetField("Description", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var isDisplayedField = role.GetField("IsDisplayed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var isRegisterableField = role.GetField("IsRegisterable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (idField?.FieldType != typeof(string) || nameField?.FieldType != typeof(string) || descriptionField?.FieldType != typeof(string) || isDisplayedField?.FieldType != typeof(bool) || isRegisterableField?.FieldType != typeof(bool))
            {
                continue;
            }

            var roleIdentifier = (string)(idField.GetValue(null) ?? throw new NotFoundException($"Role identifier could not be found for {role.Name}."));
            var roleName = (string)(nameField.GetValue(null) ?? throw new NotFoundException($"Role name could not be found for {role.Name}."));
            var roleDescription = (string)(descriptionField.GetValue(null) ?? throw new NotFoundException($"Role description could not be found for {role.Name}."));
            var roleDisplayedFlag = (bool)(isDisplayedField.GetValue(null) ?? throw new NotFoundException($"Role displayed flag could not be found for {role.Name}."));
            var roleRegisteredFlag = (bool)(isRegisterableField.GetValue(null) ?? throw new NotFoundException($"Role registration flag could not be found for {role.Name}."));

            if (!Guid.TryParse(roleIdentifier, out var id)) continue;
            if (string.IsNullOrWhiteSpace(roleName)) continue;
            if (string.IsNullOrWhiteSpace(roleDescription)) continue;

            roleSpecifications.Add(new RoleSpecification(id, roleName, roleDescription, roleDisplayedFlag, roleRegisteredFlag));
        }

        return roleSpecifications;
    }
}
