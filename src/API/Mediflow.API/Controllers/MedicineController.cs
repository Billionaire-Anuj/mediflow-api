using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Medicines;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class MedicineController(IMedicineService medicineService) : BaseController<MedicineController>
{
    [HttpGet]
    [Documentation("GetAllMedicines", "Retrieve all paginated medicines in the system.")]
    public CollectionDto<MedicineDto> GetAllMedicines(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] List<Guid>? medicationTypeIds = null,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null,
        [FromQuery] string? format = null)
    {
        var result = medicineService.GetAllMedicines(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            medicationTypeIds,
            title,
            description,
            format);

        return new CollectionDto<MedicineDto>(
            (int)HttpStatusCode.OK,
            "The medicines have been successfully retrieved.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Documentation("GetAllMedicinesList", "Retrieve all non-paginated medicines in the system.")]
    public ResponseDto<List<MedicineDto>> GetAllMedicines(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] List<Guid>? medicationTypeIds = null,
        [FromQuery] string? title = null,
        [FromQuery] string? description = null,
        [FromQuery] string? format = null)
    {
        var result = medicineService.GetAllMedicines(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            medicationTypeIds,
            title,
            description,
            format);

        return new ResponseDto<List<MedicineDto>>(
            (int)HttpStatusCode.OK,
            "The medicines have been successfully retrieved.",
            result);
    }

    [HttpGet("{medicineId:guid}")]
    [Documentation("GetMedicineById", "Retrieve the respective medicine via its identifier in the system.")]
    public ResponseDto<MedicineDto> GetMedicineById([FromRoute] Guid medicineId)
    {
        var result = medicineService.GetMedicineById(medicineId);

        return new ResponseDto<MedicineDto>(
            (int)HttpStatusCode.OK,
            "Medicine successfully fetched.",
            result);
    }

    [HttpPost]
    [Documentation("CreateMedicine", "Creates a new record of medicine.")]
    public ResponseDto<bool> CreateMedicine([FromBody] CreateMedicineDto medicine)
    {
        medicineService.CreateMedicine(medicine);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Medicine successfully created.",
            true);
    }

    [HttpPut("{medicineId:guid}")]
    [Documentation("UpdateMedicine", "Updates an existing record of medicine.")]
    public ResponseDto<bool> UpdateMedicine([FromRoute] Guid medicineId, [FromBody] UpdateMedicineDto medicine)
    {
        medicineService.UpdateMedicine(medicineId, medicine);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Medicine successfully updated.",
            true);
    }

    [HttpPatch("{medicineId:guid}/status")]
    [Documentation("ActivateDeactivateMedicine", "Updates the activation status of a medicine.")]
    public ResponseDto<bool> ActivateDeactivateMedicine([FromRoute] Guid medicineId)
    {
        medicineService.ActivateDeactivateMedicine(medicineId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Medicine status successfully updated.",
            true);
    }
}
