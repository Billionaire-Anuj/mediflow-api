using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.MedicationTypes;

namespace Mediflow.Application.Interfaces.Services;

public interface IMedicationTypeService : ITransientService
{
    List<MedicationTypeDto> GetAllMedicationTypes(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null
    );

    List<MedicationTypeDto> GetAllMedicationTypes(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null
    );

    MedicationTypeDto GetMedicationTypeById(Guid medicationTypeId);

    void CreateMedicationType(CreateMedicationTypeDto medicationType);

    void UpdateMedicationType(Guid medicationTypeId, UpdateMedicationTypeDto medicationType);

    void ActivateDeactivateMedicationType(Guid medicationTypeId);
}
