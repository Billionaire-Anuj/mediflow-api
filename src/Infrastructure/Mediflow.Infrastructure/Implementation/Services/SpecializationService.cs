using Mediflow.Domain.Entities;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Specializations;

namespace Mediflow.Infrastructure.Implementation.Services;

public class SpecializationService(IApplicationDbContext applicationDbContext) : ISpecializationService
{
    public List<SpecializationDto> GetAllSpecializations(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null)
    {
        var specializationModels = applicationDbContext.Specializations
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys);

        rowCount = specializationModels.Count();

        return specializationModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToSpecializationDto())
            .ToList();
    }

    public List<SpecializationDto> GetAllSpecializations(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null)
    {
        var specializationModels = applicationDbContext.Specializations
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys);

        return specializationModels.Select(x => x.ToSpecializationDto()).ToList();
    }

    public SpecializationDto GetSpecializationById(Guid specializationId)
    {
        var specializationModel = applicationDbContext.Specializations
            .FirstOrDefault(x => x.Id == specializationId)
            ?? throw new NotFoundException($"Specialization with the identifier of {specializationId} could not be found.");

        return specializationModel.ToSpecializationDto();
    }

    public void CreateSpecialization(CreateSpecializationDto specialization)
    {
        var duplicateSpecialization = applicationDbContext.Specializations
            .FirstOrDefault(x => x.Title.ToLower() == specialization.Title.ToLower());

        if (duplicateSpecialization != null)
            throw new BadRequestException("A specialization with the same title already exists.");

        var specializationModel = new Specialization(specialization.Title, specialization.Description);

        applicationDbContext.Specializations.Add(specializationModel);

        applicationDbContext.SaveChanges();
    }

    public void UpdateSpecialization(Guid specializationId, UpdateSpecializationDto specialization)
    {
        if (specializationId != specialization.Id)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var specializationModel = applicationDbContext.Specializations
            .FirstOrDefault(x => x.Id == specializationId)
            ?? throw new NotFoundException($"Specialization with the identifier of {specializationId} could not be found.");

        var duplicateSpecialization = applicationDbContext.Specializations
            .FirstOrDefault(x => x.Title.ToLower() == specialization.Title.ToLower() && x.Id != specialization.Id);

        if (duplicateSpecialization != null)
            throw new BadRequestException("A specialization with the same title already exists.");

        specializationModel.Update(specialization.Title, specialization.Description);

        applicationDbContext.SaveChanges();
    }

    public void ActivateDeactivateSpecialization(Guid specializationId)
    {
        var specializationModel = applicationDbContext.Specializations
            .FirstOrDefault(x => x.Id == specializationId)
            ?? throw new NotFoundException($"Specialization with the identifier of {specializationId} could not be found.");

        specializationModel.ActivateDeactivateEntity();

        applicationDbContext.SaveChanges();
    }
}
