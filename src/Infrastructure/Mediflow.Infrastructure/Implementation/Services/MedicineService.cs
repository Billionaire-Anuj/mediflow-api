using Mediflow.Domain.Entities;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Medicines;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Implementation.Services;

public class MedicineService(IApplicationDbContext applicationDbContext) : IMedicineService
{
    public List<MedicineDto> GetAllMedicines(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? medicationTypeIds = null,
        string? title = null,
        string? description = null,
        string? format = null)
    {
        var medicationTypeIdentifiers = medicationTypeIds != null ? new HashSet<Guid>(medicationTypeIds) : null;

        var medicineModels = applicationDbContext.Medicines
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())
                    || x.Format.ToLower().Contains(globalSearch.ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (medicationTypeIdentifiers == null || medicationTypeIdentifiers.Contains(x.MedicationTypeId)) &&
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())) &&
                (format == null || x.Format.ToLower().Contains(format.ToLower())))
            .Include(x => x.MedicationType)
            .OrderBy(x => orderBys);

        rowCount = medicineModels.Count();

        return medicineModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToMedicineDto())
            .ToList();
    }

    public List<MedicineDto> GetAllMedicines(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? medicationTypeIds = null,
        string? title = null,
        string? description = null,
        string? format = null)
    {
        var medicationTypeIdentifiers = medicationTypeIds != null ? new HashSet<Guid>(medicationTypeIds) : null;

        var medicineModels = applicationDbContext.Medicines
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())
                    || x.Format.ToLower().Contains(globalSearch.ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (medicationTypeIdentifiers == null || medicationTypeIdentifiers.Contains(x.MedicationTypeId)) &&
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())) &&
                (format == null || x.Format.ToLower().Contains(format.ToLower())))
            .OrderBy(x => orderBys);

        return medicineModels.Select(x => x.ToMedicineDto()).ToList();
    }

    public MedicineDto GetMedicineById(Guid medicineId)
    {
        var medicineModel = applicationDbContext.Medicines
            .FirstOrDefault(x => x.Id == medicineId)
            ?? throw new NotFoundException($"Medicine with the identifier of {medicineId} could not be found.");

        return medicineModel.ToMedicineDto();
    }

    public void CreateMedicine(CreateMedicineDto medicine)
    {
        var duplicateMedicine = applicationDbContext.Medicines
            .FirstOrDefault(x => x.Title.ToLower() == medicine.Title.ToLower());

        if (duplicateMedicine != null)
            throw new BadRequestException("A medicine with the same title already exists.");

        var medicationTypeModel = applicationDbContext.MedicationTypes
            .FirstOrDefault(x => x.Id == medicine.MedicationTypeId)
            ?? throw new NotFoundException($"Medication type with the identifier of {medicine.MedicationTypeId} could not be found.");

        var medicineModel = new Medicine(medicationTypeModel.Id, medicine.Title, medicine.Description, medicine.Format);

        applicationDbContext.Medicines.Add(medicineModel);

        applicationDbContext.SaveChanges();
    }

    public void UpdateMedicine(Guid medicineId, UpdateMedicineDto medicine)
    {
        if (medicineId != medicine.Id)
            throw new BadRequestException($"Medicine with the identifier of {medicineId} could not be found.");

        var medicineModel = applicationDbContext.Medicines
            .FirstOrDefault(x => x.Id == medicineId)
            ?? throw new NotFoundException($"Medicine with the identifier of {medicineId} could not be found.");

        var duplicateMedicine = applicationDbContext.Medicines
            .FirstOrDefault(x => x.Title.ToLower() == medicine.Title.ToLower() && x.Id != medicine.Id);

        if (duplicateMedicine != null)
            throw new BadRequestException("A medicine with the same title already exists.");

        var medicationTypeModel = applicationDbContext.MedicationTypes
            .FirstOrDefault(x => x.Id == medicine.MedicationTypeId)
            ?? throw new NotFoundException($"Medication type with the identifier of {medicine.MedicationTypeId} could not be found.");

        medicineModel.Update(medicationTypeModel.Id, medicine.Title, medicine.Description, medicine.Format);

        applicationDbContext.SaveChanges();
    }

    public void ActivateDeactivateMedicine(Guid medicineId)
    {
        var medicineModel = applicationDbContext.Medicines
            .FirstOrDefault(x => x.Id == medicineId)
            ?? throw new NotFoundException($"Medicine with the identifier of {medicineId} could not be found.");

        medicineModel.ActivateDeactivateEntity();

        applicationDbContext.SaveChanges();
    }
}
