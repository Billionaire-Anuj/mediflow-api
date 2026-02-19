using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.MedicationTypes;

public static class MedicationTypeExtensionMethods
{
    public static MedicationTypeDto ToMedicationTypeDto(this MedicationType medicationType)
    {
        return new MedicationTypeDto
        {
            Id = medicationType.Id,
            Title = medicationType.Title,
            IsActive = medicationType.IsActive,
            Description = medicationType.Description
        };
    }
}
