using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Recommendations;

namespace Mediflow.Application.Interfaces.Services;

public interface IDoctorRecommendationService : ITransientService
{
    DoctorRecommendationResultDto GetRecommendations(
        string query,
        string? city = null,
        int limit = 5);
}
