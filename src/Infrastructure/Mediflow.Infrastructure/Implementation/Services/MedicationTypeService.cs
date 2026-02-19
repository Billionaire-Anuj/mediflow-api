using Mediflow.Domain.Entities;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.MedicationTypes;

namespace Mediflow.Infrastructure.Implementation.Services;

public class MedicationTypeService(IApplicationDbContext applicationDbContext) : IMedicationTypeService
{
    public List<MedicationTypeDto> GetAllMedicationTypes(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null)
    {
        var medicationTypeModels = applicationDbContext.MedicationTypes
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys);

        rowCount = medicationTypeModels.Count();

        return medicationTypeModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToMedicationTypeDto())
            .ToList();
    }

    public List<MedicationTypeDto> GetAllMedicationTypes(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null)
    {
        var medicationTypeModels = applicationDbContext.MedicationTypes
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys);

        return medicationTypeModels.Select(x => x.ToMedicationTypeDto()).ToList();
    }

    public MedicationTypeDto GetMedicationTypeById(Guid medicationTypeId)
    {
        var medicationTypeModel = applicationDbContext.MedicationTypes
            .FirstOrDefault(x => x.Id == medicationTypeId)
            ?? throw new NotFoundException($"Medication type with the identifier of {medicationTypeId} could not be found.");

        return medicationTypeModel.ToMedicationTypeDto();
    }

    public void CreateMedicationType(CreateMedicationTypeDto medicationType)
    {
        var duplicateMedicationType = applicationDbContext.MedicationTypes
            .FirstOrDefault(x => x.Title.ToLower() == medicationType.Title.ToLower());

        if (duplicateMedicationType != null)
            throw new BadRequestException("A medication type with the same title already exists.");

        var medicationTypeModel = new MedicationType(medicationType.Title, medicationType.Description);

        applicationDbContext.MedicationTypes.Add(medicationTypeModel);

        applicationDbContext.SaveChanges();
    }

    public void UpdateMedicationType(Guid medicationTypeId, UpdateMedicationTypeDto medicationType)
    {
        if (medicationTypeId != medicationType.Id)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var medicationTypeModel = applicationDbContext.MedicationTypes
            .FirstOrDefault(x => x.Id == medicationTypeId)
            ?? throw new NotFoundException($"Medication type with the identifier of {medicationTypeId} could not be found.");

        var duplicateMedicationType = applicationDbContext.MedicationTypes
            .FirstOrDefault(x => x.Title.ToLower() == medicationType.Title.ToLower() && x.Id != medicationType.Id);

        if (duplicateMedicationType != null)
            throw new BadRequestException("A medication type with the same title already exists.");

        medicationTypeModel.Update(medicationType.Title, medicationType.Description);

        applicationDbContext.SaveChanges();
    }

    public void ActivateDeactivateMedicationType(Guid medicationTypeId)
    {
        var medicationTypeModel = applicationDbContext.MedicationTypes
            .FirstOrDefault(x => x.Id == medicationTypeId)
            ?? throw new NotFoundException($"Medication type with the identifier of {medicationTypeId} could not be found.");

        medicationTypeModel.ActivateDeactivateEntity();

        applicationDbContext.SaveChanges();
    }
}
