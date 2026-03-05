using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.DTOs.Patients;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Payments;

namespace Mediflow.API.Controllers;

public class PatientController(
    IPatientService patientService,
    IPatientCreditService patientCreditService) : BaseController<PatientController>
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

    [HttpPost("credits/khalti/initiate")]
    [Documentation("InitiateKhaltiTopup", "Initiate a Khalti payment for patient credits.")]
    public async Task<ResponseDto<KhaltiInitiateResponseDto>> InitiateKhaltiTopup([FromBody] CreditTopupRequestDto request)
    {
        var result = await patientCreditService.InitiateKhaltiTopupAsync(request);

        return new ResponseDto<KhaltiInitiateResponseDto>(
            (int)HttpStatusCode.OK,
            "Khalti payment initiated successfully.",
            result);
    }

    [HttpPost("credits/khalti/confirm")]
    [Documentation("ConfirmKhaltiTopup", "Confirm a Khalti payment and apply credits.")]
    public async Task<ResponseDto<bool>> ConfirmKhaltiTopup([FromBody] KhaltiConfirmDto request)
    {
        var result = await patientCreditService.ConfirmKhaltiTopupAsync(request);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Khalti payment verified and credits added.",
            result);
    }

    [HttpPost("credits/esewa/initiate")]
    [Documentation("InitiateEsewaTopup", "Initiate an eSewa payment for patient credits.")]
    public async Task<ResponseDto<EsewaInitiateResponseDto>> InitiateEsewaTopup([FromBody] CreditTopupRequestDto request)
    {
        var result = await patientCreditService.InitiateEsewaTopupAsync(request);

        return new ResponseDto<EsewaInitiateResponseDto>(
            (int)HttpStatusCode.OK,
            "eSewa payment initiated successfully.",
            result);
    }

    [HttpPost("credits/esewa/confirm")]
    [Documentation("ConfirmEsewaTopup", "Confirm an eSewa payment and apply credits.")]
    public async Task<ResponseDto<bool>> ConfirmEsewaTopup([FromBody] EsewaConfirmDto request)
    {
        var result = await patientCreditService.ConfirmEsewaTopupAsync(request);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "eSewa payment verified and credits added.",
            result);
    }
}
