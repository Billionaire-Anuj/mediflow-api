namespace Mediflow.Application.DTOs.Recommendations;

public class DoctorRecommendationAssessmentDto
{
    public int? AgeYears { get; set; }

    public int? SystolicBloodPressure { get; set; }

    public int? DiastolicBloodPressure { get; set; }

    public decimal? TemperatureCelsius { get; set; }

    public int? HeartRateBpm { get; set; }

    public int? RespiratoryRatePerMinute { get; set; }

    public int? OxygenSaturationPercent { get; set; }

    public decimal? BloodSugarMgDl { get; set; }

    public int Limit { get; set; } = 6;

    public List<string> Symptoms { get; set; } = new();
}
