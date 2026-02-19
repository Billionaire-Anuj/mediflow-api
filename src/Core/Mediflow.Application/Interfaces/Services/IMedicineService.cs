using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Medicines;

namespace Mediflow.Application.Interfaces.Services;

public interface IMedicineService : ITransientService
{
    List<MedicineDto> GetAllMedicines(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? medicationTypeIds = null,
        string? title = null,
        string? description = null,
        string? format = null
    );

    List<MedicineDto> GetAllMedicines(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? medicationTypeIds = null,
        string? title = null,
        string? description = null,
        string? format = null
    );

    MedicineDto GetMedicineById(Guid medicineId);

    void CreateMedicine(CreateMedicineDto medicine);

    void UpdateMedicine(Guid medicineId, UpdateMedicineDto medicine);

    void ActivateDeactivateMedicine(Guid medicineId);
}
