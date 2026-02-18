using System.Reflection;
using Mediflow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;
using Mediflow.Application.Common.Authorization;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class ResourcesSeeder(ILogger<ResourcesSeeder> logger, IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public int Order => 20;

    public void Seed()
    {
        var resources = typeof(MediflowResources)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.FieldType == typeof(string))
            .Select(x => x.GetValue(null)!.ToString()!)
            .AsQueryable();

        var resourceModels = applicationDbContext.Resources.AsQueryable();

        foreach (var resource in resources)
        {
            if (applicationDbContext.Resources.Any(x => x.Name == resource))
            {
                logger.LogDebug("Resource with name '{ResourceName}' already exists.", resource);
                continue;
            }

            var resourceModel = new Resource(resource, $"{resource} Description.");

            applicationDbContext.Resources.Add(resourceModel);

            logger.LogInformation("Seeding resource '{ResourceName}'.", resource);
        }

        foreach (var resource in resourceModels)
        {
            if (resources.Contains(resource.Name)) continue;

            var permissions = applicationDbContext.Permissions.Where(x => x.ResourceId == resource.Id).AsQueryable();

            if (permissions.Any())
            {
                applicationDbContext.Permissions.RemoveRange(permissions);

                logger.LogInformation("Removed permissions for resource '{ResourceName}' as it no longer exists in internal resources.", resource.Name);
            }

            applicationDbContext.Resources.Remove(resource);

            logger.LogInformation("Deleted resource '{ResourceName}' as it no longer exists in internal resources.", resource.Name);
        }

        applicationDbContext.SaveChanges();

        logger.LogInformation("Resources initialization successfully completed.");
    }
}
