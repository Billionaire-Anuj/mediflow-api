using Mediflow.Domain.Entities;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.DiagnosticTests;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Implementation.Services;

public class DiagnosticTestService(IApplicationDbContext applicationDbContext) : IDiagnosticTestService
{
    public List<DiagnosticTestDto> GetAllDiagnosticTests(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? diagnosticTypeIds = null,
        string? title = null,
        string? description = null,
        string? specimen = null)
    {
        var diagnosticTypeIdentifiers = diagnosticTypeIds != null ? new HashSet<Guid>(diagnosticTypeIds) : null;

        var diagnosticTestModels = applicationDbContext.DiagnosticTests
            .Where(x => 
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())
                    || x.Specimen.ToLower().Contains(globalSearch.ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (diagnosticTypeIdentifiers == null || diagnosticTypeIdentifiers.Contains(x.DiagnosticTypeId)) &&
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())) && 
                (specimen == null || x.Specimen.ToLower().Contains(specimen.ToLower())))
            .Include(x => x.DiagnosticType)
            .OrderBy(x => orderBys);

        rowCount = diagnosticTestModels.Count();

        return diagnosticTestModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToDiagnosticTestDto())
            .ToList();
    }

    public List<DiagnosticTestDto> GetAllDiagnosticTests(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        List<Guid>? diagnosticTypeIds = null,
        string? title = null,
        string? description = null,
        string? specimen = null)
    {
        var diagnosticTypeIdentifiers = diagnosticTypeIds != null ? new HashSet<Guid>(diagnosticTypeIds) : null;

        var diagnosticTestModels = applicationDbContext.DiagnosticTests
            .Where(x => 
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Title.ToLower().Contains(globalSearch.ToLower())
                    || x.Description.ToLower().Contains(globalSearch.ToLower())
                    || x.Specimen.ToLower().Contains(globalSearch.ToLower())) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (diagnosticTypeIdentifiers == null || diagnosticTypeIdentifiers.Contains(x.DiagnosticTypeId)) &&
                (title == null || x.Title.ToLower().Contains(title.ToLower())) &&
                (description == null || x.Description.ToLower().Contains(description.ToLower())) && 
                (specimen == null || x.Specimen.ToLower().Contains(specimen.ToLower())))
            .Include(x => x.DiagnosticType)
            .OrderBy(x => orderBys);

        return diagnosticTestModels.Select(x => x.ToDiagnosticTestDto()).ToList();
    }

    public DiagnosticTestDto GetDiagnosticTestById(Guid diagnosticTestId)
    {
        var diagnosticTestModel = applicationDbContext.DiagnosticTests
            .FirstOrDefault(x => x.Id == diagnosticTestId)
            ?? throw new NotFoundException($"Diagnostic type with the identifier of {diagnosticTestId} could not be found.");

        return diagnosticTestModel.ToDiagnosticTestDto();
    }

    public void CreateDiagnosticTest(CreateDiagnosticTestDto diagnosticTest)
    {
        var duplicateDiagnosticTest = applicationDbContext.DiagnosticTests
            .FirstOrDefault(x => x.Title.ToLower() == diagnosticTest.Title.ToLower());

        if (duplicateDiagnosticTest != null)
            throw new BadRequestException("A diagnostic type with the same title already exists.");

        var diagnosticTypeModel = applicationDbContext.DiagnosticTypes
            .FirstOrDefault(x => x.Id == diagnosticTest.DiagnosticTypeId)
            ?? throw new NotFoundException($"Diagnostic type with the identifier of {diagnosticTest.DiagnosticTypeId} could not be found.");

        var diagnosticTestModel = new DiagnosticTest(diagnosticTypeModel.Id, diagnosticTest.Title, diagnosticTest.Description, diagnosticTest.Specimen);

        applicationDbContext.DiagnosticTests.Add(diagnosticTestModel);

        applicationDbContext.SaveChanges();
    }

    public void UpdateDiagnosticTest(Guid diagnosticTestId, UpdateDiagnosticTestDto diagnosticTest)
    {
        if (diagnosticTestId != diagnosticTest.Id)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var diagnosticTestModel = applicationDbContext.DiagnosticTests
            .FirstOrDefault(x => x.Id == diagnosticTestId)
            ?? throw new NotFoundException($"Diagnostic type with the identifier of {diagnosticTestId} could not be found.");

        var duplicateDiagnosticTest = applicationDbContext.DiagnosticTests
            .FirstOrDefault(x => x.Title.ToLower() == diagnosticTest.Title.ToLower() && x.Id != diagnosticTest.Id);

        if (duplicateDiagnosticTest != null)
            throw new BadRequestException("A diagnostic type with the same title already exists.");

        var diagnosticTypeModel = applicationDbContext.DiagnosticTypes
            .FirstOrDefault(x => x.Id == diagnosticTest.DiagnosticTypeId)
            ?? throw new NotFoundException($"Diagnostic type with the identifier of {diagnosticTest.DiagnosticTypeId} could not be found.");

        diagnosticTestModel.Update(diagnosticTypeModel.Id, diagnosticTest.Title, diagnosticTest.Description, diagnosticTest.Specimen);

        applicationDbContext.SaveChanges();
    }

    public void ActivateDeactivateDiagnosticTest(Guid diagnosticTestId)
    {
        var diagnosticTestModel = applicationDbContext.DiagnosticTests
                                      .FirstOrDefault(x => x.Id == diagnosticTestId)
                                  ?? throw new NotFoundException($"Diagnostic type with the identifier of {diagnosticTestId} could not be found.");

        diagnosticTestModel.ActivateDeactivateEntity();

        applicationDbContext.SaveChanges();
    }
}