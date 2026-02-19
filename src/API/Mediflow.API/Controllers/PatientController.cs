using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.DTOs.Patients;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class PatientController(IPatientService patientService) : BaseController<PatientController>
{
    [HttpGet("profile")]
    [Documentation("GetPatientProfile", "Retrieve the logged in patient's profile.")]
    public ResponseDto<PatientProfileDto> GetPatientProfile()
    {
        var result = patientService.GetPatientProfile();

        return new ResponseDto<PatientProfileDto>(
            (int)HttpStatusCode.OK,
            "Patient profile successfully fetched.",
            result);
    }

    [HttpGet("{patientId:guid}")]
    [Documentation("GetPatientProfileById", "Retrieve the respective patient profile via its identifier in the system.")]
    public ResponseDto<PatientProfileDto> GetPatientProfileById([FromRoute] Guid patientId)
    {
        var result = patientService.GetPatientProfile(patientId);

        return new ResponseDto<PatientProfileDto>(
            (int)HttpStatusCode.OK,
            "Patient profile successfully fetched.",
            result);
    }
}
