using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.DiagnosticTypes;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class DiagnosticTypeController(IDiagnosticTypeService diagnosticTypeService) : BaseController<DiagnosticTypeController>
{
    [HttpGet]
    [Documentation("GetAllDiagnosticTypes", "Retrieve all paginated diagnostic types in the system.")]
    public CollectionDto<DiagnosticTypeDto> GetAllDiagnosticTypes(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null)
    {
        var result = diagnosticTypeService.GetAllDiagnosticTypes(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            title,
            description);

        return new CollectionDto<DiagnosticTypeDto>(
            (int)HttpStatusCode.OK,
            "The diagnostic types have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllDiagnosticTypesList", "Retrieve all non-paginated diagnostic types in the system.")]
    public ResponseDto<List<DiagnosticTypeDto>> GetAllDiagnosticTypes(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null)
    {
        var result = diagnosticTypeService.GetAllDiagnosticTypes(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            title,
            description);

        return new ResponseDto<List<DiagnosticTypeDto>>(
            (int)HttpStatusCode.OK,
            "The diagnostic types have been successfully retrieved.",
            result);
    }

    [HttpGet("{diagnosticTypeId:guid}")]
    [Documentation("GetDiagnosticTypeById", "Retrieve the respective diagnosticType via its identifier in the system.")]
    public ResponseDto<DiagnosticTypeDto> GetDiagnosticTypeById([FromRoute] Guid diagnosticTypeId)
    {
        var result = diagnosticTypeService.GetDiagnosticTypeById(diagnosticTypeId);

        return new ResponseDto<DiagnosticTypeDto>(
            (int)HttpStatusCode.OK,
            "Diagnostic type successfully fetched.",
            result);
    }

    [HttpPost]
    [Documentation("CreateDiagnosticType", "Creates a new record of diagnosticType.")]
    public ResponseDto<bool> CreateDiagnosticType([FromBody] CreateDiagnosticTypeDto diagnosticType)
    {
        diagnosticTypeService.CreateDiagnosticType(diagnosticType);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Diagnostic type successfully created.",
            true);
    }

    [HttpPut("{diagnosticTypeId:guid}")]
    [Documentation("UpdateDiagnosticType", "Updates an existing record of diagnosticType.")]
    public ResponseDto<bool> UpdateDiagnosticType([FromRoute] Guid diagnosticTypeId, [FromBody] UpdateDiagnosticTypeDto diagnosticType)
    {
        diagnosticTypeService.UpdateDiagnosticType(diagnosticTypeId, diagnosticType);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Diagnostic type successfully updated.",
            true);
    }

    [HttpPatch("{diagnosticTypeId:guid}/status")]
    [Documentation("ActivateDeactivateDiagnosticType", "Updates the activation status of a diagnosticType.")]
    public ResponseDto<bool> ActivateDeactivateDiagnosticType([FromRoute] Guid diagnosticTypeId)
    {
        diagnosticTypeService.ActivateDeactivateDiagnosticType(diagnosticTypeId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Diagnostic type status successfully updated.",
            true);
    }
}