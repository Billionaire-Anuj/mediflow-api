using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Specializations;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class SpecializationController(ISpecializationService specializationService) : BaseController<SpecializationController>
{
    [HttpGet]
    [Documentation("GetAllSpecializations", "Retrieve all paginated specializations in the system.")]
    public CollectionDto<SpecializationDto> GetAllSpecializations(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null)
    {
        var result = specializationService.GetAllSpecializations(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            title,
            description);

        return new CollectionDto<SpecializationDto>(
            (int)HttpStatusCode.OK,
            "The specializations have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllSpecializationsList", "Retrieve all non-paginated specializations in the system.")]
    public ResponseDto<List<SpecializationDto>> GetAllSpecializations(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null)
    {
        var result = specializationService.GetAllSpecializations(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            title,
            description);

        return new ResponseDto<List<SpecializationDto>>(
            (int)HttpStatusCode.OK,
            "The specializations have been successfully retrieved.",
            result);
    }

    [HttpGet("{specializationId:guid}")]
    [Documentation("GetSpecializationById", "Retrieve the respective specialization via its identifier in the system.")]
    public ResponseDto<SpecializationDto> GetSpecializationById([FromRoute] Guid specializationId)
    {
        var result = specializationService.GetSpecializationById(specializationId);

        return new ResponseDto<SpecializationDto>(
            (int)HttpStatusCode.OK,
            "Specialization successfully fetched.",
            result);
    }

    [HttpPost]
    [Documentation("CreateSpecialization", "Creates a new record of specialization.")]
    public ResponseDto<bool> CreateSpecialization([FromBody] CreateSpecializationDto specialization)
    {
        specializationService.CreateSpecialization(specialization);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Specialization successfully created.",
            true);
    }

    [HttpPut("{specializationId:guid}")]
    [Documentation("UpdateSpecialization", "Updates an existing record of specialization.")]
    public ResponseDto<bool> UpdateSpecialization([FromRoute] Guid specializationId, [FromBody] UpdateSpecializationDto specialization)
    {
        specializationService.UpdateSpecialization(specializationId, specialization);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Specialization successfully updated.",
            true);
    }

    [HttpPatch("{specializationId:guid}/status")]
    [Documentation("ActivateDeactivateSpecialization", "Updates the activation status of a specialization.")]
    public ResponseDto<bool> ActivateDeactivateSpecialization([FromRoute] Guid specializationId)
    {
        specializationService.ActivateDeactivateSpecialization(specializationId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Specialization status successfully updated.",
            true);
    }
}
