using Mediflow.Domain.Entities;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.DiagnosticTypes;

namespace Mediflow.Infrastructure.Implementation.Services;

public class DiagnosticTypeService(IApplicationDbContext applicationDbContext) : IDiagnosticTypeService
{
    public List<DiagnosticTypeDto> GetAllDiagnosticTypes(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null)
    {
        var diagnosticTypeModels = applicationDbContext.DiagnosticTypes
            .Where(x => 
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys);

        rowCount = diagnosticTypeModels.Count();

        return diagnosticTypeModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToDiagnosticTypeDto())
            .ToList();
    }

    public List<DiagnosticTypeDto> GetAllDiagnosticTypes(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? title = null,
        string? description = null)
    {
        var diagnosticTypeModels = applicationDbContext.DiagnosticTypes
            .Where(x => 
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())))
            .OrderBy(x => orderBys);

        return diagnosticTypeModels.Select(x => x.ToDiagnosticTypeDto()).ToList();
    }

    public DiagnosticTypeDto GetDiagnosticTypeById(Guid diagnosticTypeId)
    {
        var diagnosticTypeModel = applicationDbContext.DiagnosticTypes
            .FirstOrDefault(x => x.Id == diagnosticTypeId)
            ?? throw new NotFoundException($"Diagnostic type with the identifier of {diagnosticTypeId} could not be found.");

        return diagnosticTypeModel.ToDiagnosticTypeDto();
    }

    public void CreateDiagnosticType(CreateDiagnosticTypeDto diagnosticType)
    {
        var duplicateDiagnosticType = applicationDbContext.DiagnosticTypes
            .FirstOrDefault(x => x.Title.ToLower() == diagnosticType.Title.ToLower());

        if (duplicateDiagnosticType != null)
            throw new BadRequestException("A diagnostic type with the same title already exists.");

        var diagnosticTypeModel = new DiagnosticType(diagnosticType.Title, diagnosticType.Description);

        applicationDbContext.DiagnosticTypes.Add(diagnosticTypeModel);

        applicationDbContext.SaveChanges();
    }

    public void UpdateDiagnosticType(Guid diagnosticTypeId, UpdateDiagnosticTypeDto diagnosticType)
    {
        if (diagnosticTypeId != diagnosticType.Id)
            throw new BadRequestException($"Diagnostic type with the identifier of {diagnosticTypeId} could not be found.");

        var diagnosticTypeModel = applicationDbContext.DiagnosticTypes
            .FirstOrDefault(x => x.Id == diagnosticTypeId)
            ?? throw new NotFoundException($"Diagnostic type with the identifier of {diagnosticTypeId} could not be found.");

        var duplicateDiagnosticType = applicationDbContext.DiagnosticTypes
            .FirstOrDefault(x => x.Title.ToLower() == diagnosticType.Title.ToLower() && x.Id != diagnosticType.Id);

        if (duplicateDiagnosticType != null)
            throw new BadRequestException("A diagnostic type with the same title already exists.");

        diagnosticTypeModel.Update(diagnosticType.Title, diagnosticType.Description);

        applicationDbContext.SaveChanges();
    }

    public void ActivateDeactivateDiagnosticType(Guid diagnosticTypeId)
    {
        var diagnosticTypeModel = applicationDbContext.DiagnosticTypes
                                      .FirstOrDefault(x => x.Id == diagnosticTypeId)
                                  ?? throw new NotFoundException($"Diagnostic type with the identifier of {diagnosticTypeId} could not be found.");

        diagnosticTypeModel.ActivateDeactivateEntity();

        applicationDbContext.SaveChanges();
    }
}