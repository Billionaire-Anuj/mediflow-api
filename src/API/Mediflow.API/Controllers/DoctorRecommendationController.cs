using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Recommendations;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class DoctorRecommendationController(IDoctorRecommendationService doctorRecommendationService) : BaseController<DoctorRecommendationController>
{
    [HttpGet]
    [Documentation("GetDoctorRecommendations", "Recommend doctors based on user input (specialization or keywords).")]
    public ResponseDto<DoctorRecommendationResultDto> GetDoctorRecommendations([FromQuery] string query, [FromQuery] string? city = null, [FromQuery] int limit = 5)
    {
        var result = doctorRecommendationService.GetRecommendations(query, city, limit);

        return new ResponseDto<DoctorRecommendationResultDto>(
            (int)HttpStatusCode.OK,
            "Doctor recommendations successfully generated.",
            result);
    }
}
