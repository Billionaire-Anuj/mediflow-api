using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.DiagnosticTests;

namespace Mediflow.API.Controllers;

public class DiagnosticTestController(IDiagnosticTestService diagnosticTestService) : BaseController<DiagnosticTestController>
{
    [HttpGet]
    [Documentation("GetAllDiagnosticTests", "Retrieve all paginated diagnostic tests in the system.")]
    public CollectionDto<DiagnosticTestDto> GetAllDiagnosticTests(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] List<Guid>? diagnosticTypeIds = null,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null,
        [FromQuery] string? specimen = null)
    {
        var result = diagnosticTestService.GetAllDiagnosticTests(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            diagnosticTypeIds,
            title,
            description,
            specimen);

        return new CollectionDto<DiagnosticTestDto>(
            (int)HttpStatusCode.OK,
            "The diagnostic tests have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllDiagnosticTestsList", "Retrieve all non-paginated diagnostic tests in the system.")]
    public ResponseDto<List<DiagnosticTestDto>> GetAllDiagnosticTests(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] List<Guid>? diagnosticTypeIds = null,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null,
        [FromQuery] string? specimen = null)
    {
        var result = diagnosticTestService.GetAllDiagnosticTests(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            diagnosticTypeIds,
            title,
            description,
            specimen);

        return new ResponseDto<List<DiagnosticTestDto>>(
            (int)HttpStatusCode.OK,
            "The diagnostic tests have been successfully retrieved.",
            result);
    }

    [HttpGet("{diagnosticTestId:guid}")]
    [Documentation("GetDiagnosticTestById", "Retrieve the respective diagnosticTest via its identifier in the system.")]
    public ResponseDto<DiagnosticTestDto> GetDiagnosticTestById([FromRoute] Guid diagnosticTestId)
    {
        var result = diagnosticTestService.GetDiagnosticTestById(diagnosticTestId);

        return new ResponseDto<DiagnosticTestDto>(
            (int)HttpStatusCode.OK,
            "Diagnostic test successfully fetched.",
            result);
    }

    [HttpPost]
    [Documentation("CreateDiagnosticTest", "Creates a new record of diagnosticTest.")]
    public ResponseDto<bool> CreateDiagnosticTest([FromBody] CreateDiagnosticTestDto diagnosticTest)
    {
        diagnosticTestService.CreateDiagnosticTest(diagnosticTest);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Diagnostic test successfully created.",
            true);
    }

    [HttpPut("{diagnosticTestId:guid}")]
    [Documentation("UpdateDiagnosticTest", "Updates an existing record of diagnosticTest.")]
    public ResponseDto<bool> UpdateDiagnosticTest([FromRoute] Guid diagnosticTestId, [FromBody] UpdateDiagnosticTestDto diagnosticTest)
    {
        diagnosticTestService.UpdateDiagnosticTest(diagnosticTestId, diagnosticTest);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Diagnostic test successfully updated.",
            true);
    }

    [HttpPatch("{diagnosticTestId:guid}/status")]
    [Documentation("ActivateDeactivateDiagnosticTest", "Updates the activation status of a diagnosticTest.")]
    public ResponseDto<bool> ActivateDeactivateDiagnosticTest([FromRoute] Guid diagnosticTestId)
    {
        diagnosticTestService.ActivateDeactivateDiagnosticTest(diagnosticTestId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Diagnostic test status successfully updated.",
            true);
    }
}