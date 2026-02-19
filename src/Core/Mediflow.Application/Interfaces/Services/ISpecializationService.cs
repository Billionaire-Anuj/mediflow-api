using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Specializations;

namespace Mediflow.Application.Interfaces.Services;

public interface ISpecializationService : ITransientService
{
    List<SpecializationDto> GetAllSpecializations(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null
    );

    List<SpecializationDto> GetAllSpecializations(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null
    );

    SpecializationDto GetSpecializationById(Guid specializationId);

    void CreateSpecialization(CreateSpecializationDto specialization);

    void UpdateSpecialization(Guid specializationId, UpdateSpecializationDto specialization);

    void ActivateDeactivateSpecialization(Guid specializationId);
}
