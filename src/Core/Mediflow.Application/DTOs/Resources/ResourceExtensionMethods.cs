using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Resources;

public static class ResourceExtensionMethods
{
    public static ResourceDto ToResourceDto(this Resource resources)
    {
        return new ResourceDto()
        {
            Id = resources.Id,
            Name = resources.Name,
            Description = resources.Description,
            IsActive = true
        };
    }
}