using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Specializations;

public static class SpecializationExtensionMethods
{
    public static SpecializationDto ToSpecializationDto(this Specialization specialization)
    {
        return new SpecializationDto
        {
            Id = specialization.Id,
            Title = specialization.Title,
            IsActive = specialization.IsActive,
            Description = specialization.Description
        };
    }
}
