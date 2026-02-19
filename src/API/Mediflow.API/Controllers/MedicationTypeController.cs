using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.MedicationTypes;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class MedicationTypeController(IMedicationTypeService medicationTypeService) : BaseController<MedicationTypeController>
{
    [HttpGet]
    [Documentation("GetAllMedicationTypes", "Retrieve all paginated medication types in the system.")]
    public CollectionDto<MedicationTypeDto> GetAllMedicationTypes(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null)
    {
        var result = medicationTypeService.GetAllMedicationTypes(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            title,
            description);

        return new CollectionDto<MedicationTypeDto>(
            (int)HttpStatusCode.OK,
            "The medication types have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllMedicationTypesList", "Retrieve all non-paginated medication types in the system.")]
    public ResponseDto<List<MedicationTypeDto>> GetAllMedicationTypes(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null)
    {
        var result = medicationTypeService.GetAllMedicationTypes(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            title,
            description);

        return new ResponseDto<List<MedicationTypeDto>>(
            (int)HttpStatusCode.OK,
            "The medication types have been successfully retrieved.",
            result);
    }

    [HttpGet("{medicationTypeId:guid}")]
    [Documentation("GetMedicationTypeById", "Retrieve the respective medicationType via its identifier in the system.")]
    public ResponseDto<MedicationTypeDto> GetMedicationTypeById([FromRoute] Guid medicationTypeId)
    {
        var result = medicationTypeService.GetMedicationTypeById(medicationTypeId);

        return new ResponseDto<MedicationTypeDto>(
            (int)HttpStatusCode.OK,
            "Medication type successfully fetched.",
            result);
    }

    [HttpPost]
    [Documentation("CreateMedicationType", "Creates a new record of medicationType.")]
    public ResponseDto<bool> CreateMedicationType([FromBody] CreateMedicationTypeDto medicationType)
    {
        medicationTypeService.CreateMedicationType(medicationType);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Medication type successfully created.",
            true);
    }

    [HttpPut("{medicationTypeId:guid}")]
    [Documentation("UpdateMedicationType", "Updates an existing record of medicationType.")]
    public ResponseDto<bool> UpdateMedicationType([FromRoute] Guid medicationTypeId, [FromBody] UpdateMedicationTypeDto medicationType)
    {
        medicationTypeService.UpdateMedicationType(medicationTypeId, medicationType);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Medication type successfully updated.",
            true);
    }

    [HttpPatch("{medicationTypeId:guid}/status")]
    [Documentation("ActivateDeactivateMedicationType", "Updates the activation status of a medicationType.")]
    public ResponseDto<bool> ActivateDeactivateMedicationType([FromRoute] Guid medicationTypeId)
    {
        medicationTypeService.ActivateDeactivateMedicationType(medicationTypeId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Medication type status successfully updated.",
            true);
    }
}
