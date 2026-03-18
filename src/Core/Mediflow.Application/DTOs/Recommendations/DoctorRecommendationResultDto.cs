using Mediflow.Application.DTOs.Doctors;

namespace Mediflow.Application.DTOs.Recommendations;

public class DoctorRecommendationResultDto
{
    public string Query { get; set; } = string.Empty;

    public string AssessmentSummary { get; set; } = string.Empty;

    public List<string> MatchedSignals { get; set; } = new();

    public string RecommendedSpecialization { get; set; } = string.Empty;

    public List<DoctorProfileDto> Doctors { get; set; } = new();

    public List<DoctorDirectoryDto> DatasetFallback { get; set; } = new();
}
