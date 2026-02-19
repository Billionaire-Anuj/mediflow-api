using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.MedicationTypes;

namespace Mediflow.Application.DTOs.Medicines;

public static class MedicineExtensionMethods
{
    public static MedicineDto ToMedicineDto(this Medicine medicine)
    {
        return new MedicineDto
        {
            Id = medicine.Id,
            Title = medicine.Title,
            Format = medicine.Format,
            IsActive = medicine.IsActive,
            Description = medicine.Description,
            MedicationType = (medicine.MedicationType ?? MedicationType.Default).ToMedicationTypeDto()
        };
    }
}
