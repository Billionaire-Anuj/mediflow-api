using Mediflow.Application.DTOs.Doctors;

namespace Mediflow.Application.DTOs.Recommendations;

public class DoctorRecommendationResultDto
{
    public string Query { get; set; } = string.Empty;

    public string RecommendedSpecialization { get; set; } = string.Empty;

    public List<DoctorProfileDto> Doctors { get; set; } = new();

    public List<DoctorDirectoryDto> DatasetFallback { get; set; } = new();
}
