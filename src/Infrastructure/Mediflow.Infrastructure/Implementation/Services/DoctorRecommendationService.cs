using System.Globalization;
using System.Text;
using Mediflow.Application.DTOs.Doctors;
using Mediflow.Application.DTOs.Recommendations;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Domain.Common;
using Mediflow.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Implementation.Services;

public class DoctorRecommendationService(
    IWebHostEnvironment webHostEnvironment,
    IApplicationDbContext applicationDbContext) : IDoctorRecommendationService
{
    private const string DatasetRelativePath = "data/recommendations/doctors.csv";
    private const string DefaultSpecialization = "General Physician";

    private readonly Lazy<List<DoctorDirectoryRecordDto>> _dataset = new(() => new List<DoctorDirectoryRecordDto>());

    public DoctorRecommendationResultDto GetRecommendations(string query, string? city = null, int limit = 5)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new BadRequestException("Query cannot be empty.");

        if (limit <= 0) limit = 5;

        var availableSpecializations = GetAvailableSpecializations();
        var recommendedSpecialization = ResolveSpecialization(query, availableSpecializations);
        var doctors = GetDoctorsFromDatabase(recommendedSpecialization, city, limit);

        var result = new DoctorRecommendationResultDto
        {
            Query = query,
            AssessmentSummary = $"Based on the entered concern, {recommendedSpecialization} appears to be the closest fit.",
            MatchedSignals = Tokenize(query).Take(5).ToList(),
            RecommendedSpecialization = recommendedSpecialization,
            Doctors = doctors
        };

        if (doctors.Count == 0)
        {
            result.DatasetFallback = GetDatasetFallback(recommendedSpecialization, city, limit);
        }

        return result;
    }

    public DoctorRecommendationResultDto GetRecommendations(DoctorRecommendationAssessmentDto assessment)
    {
        if (assessment == null)
            throw new BadRequestException("Assessment cannot be empty.");

        var normalizedSymptoms = (assessment.Symptoms ?? new List<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var hasVitals =
            assessment.AgeYears.HasValue ||
            assessment.SystolicBloodPressure.HasValue ||
            assessment.DiastolicBloodPressure.HasValue ||
            assessment.TemperatureCelsius.HasValue ||
            assessment.HeartRateBpm.HasValue ||
            assessment.RespiratoryRatePerMinute.HasValue ||
            assessment.OxygenSaturationPercent.HasValue ||
            assessment.BloodSugarMgDl.HasValue;

        if (!hasVitals && normalizedSymptoms.Count == 0)
            throw new BadRequestException("Please provide at least one symptom or one clinical measurement.");

        var availableSpecializations = GetAvailableSpecializations();
        var recommendation = ResolveAssessmentRecommendation(assessment, normalizedSymptoms, availableSpecializations);

        var result = new DoctorRecommendationResultDto
        {
            Query = recommendation.AssessmentSummary,
            AssessmentSummary = recommendation.AssessmentSummary,
            MatchedSignals = recommendation.Signals,
            RecommendedSpecialization = recommendation.Specialization
        };

        return result;
    }

    private AssessmentRecommendation ResolveAssessmentRecommendation(
        DoctorRecommendationAssessmentDto assessment,
        IReadOnlyCollection<string> symptoms,
        IReadOnlyCollection<string> availableSpecializations)
    {
        var scores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var signals = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        void AddScore(string specialization, int points, string signal)
        {
            if (string.IsNullOrWhiteSpace(signal)) return;

            scores[specialization] = scores.TryGetValue(specialization, out var currentScore)
                ? currentScore + points
                : points;

            if (!signals.TryGetValue(specialization, out var specializationSignals))
            {
                specializationSignals = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                signals[specialization] = specializationSignals;
            }

            specializationSignals.Add(signal);
        }

        var loweredSymptoms = symptoms
            .Select(x => x.ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        bool HasAny(params string[] values) => values.Any(loweredSymptoms.Contains);

        if (assessment.AgeYears.HasValue && assessment.AgeYears.Value > 0 && assessment.AgeYears.Value < 16)
        {
            AddScore("Pediatrician", 4, $"Age {assessment.AgeYears.Value} years");
        }

        if (assessment.OxygenSaturationPercent.HasValue && assessment.OxygenSaturationPercent.Value <= 92)
        {
            AddScore("Critical Care Physician", 8, $"Oxygen saturation {assessment.OxygenSaturationPercent.Value}%");
        }

        if (assessment.SystolicBloodPressure.HasValue && assessment.SystolicBloodPressure.Value >= 180 ||
            assessment.DiastolicBloodPressure.HasValue && assessment.DiastolicBloodPressure.Value >= 120)
        {
            AddScore("Critical Care Physician", 7, "Severely elevated blood pressure");
            AddScore("Cardiologist", 5, "Severely elevated blood pressure");
        }
        else if (assessment.SystolicBloodPressure.HasValue && assessment.SystolicBloodPressure.Value >= 140 ||
                 assessment.DiastolicBloodPressure.HasValue && assessment.DiastolicBloodPressure.Value >= 90)
        {
            AddScore("Cardiologist", 4, "Elevated blood pressure");
        }

        if (assessment.TemperatureCelsius.HasValue && assessment.TemperatureCelsius.Value >= 39.5m)
        {
            AddScore("Infectious Disease Specialist", 4, $"Temperature {assessment.TemperatureCelsius.Value:0.#} C");
            AddScore("Critical Care Physician", 3, "High fever");
        }
        else if (assessment.TemperatureCelsius.HasValue && assessment.TemperatureCelsius.Value >= 38m)
        {
            AddScore("Infectious Disease Specialist", 3, $"Temperature {assessment.TemperatureCelsius.Value:0.#} C");
        }

        if (assessment.HeartRateBpm.HasValue && assessment.HeartRateBpm.Value >= 130)
        {
            AddScore("Critical Care Physician", 5, $"Heart rate {assessment.HeartRateBpm.Value} bpm");
            AddScore("Cardiologist", 4, $"Heart rate {assessment.HeartRateBpm.Value} bpm");
        }
        else if (assessment.HeartRateBpm.HasValue && assessment.HeartRateBpm.Value >= 110)
        {
            AddScore("Cardiologist", 3, $"Heart rate {assessment.HeartRateBpm.Value} bpm");
        }

        if (assessment.RespiratoryRatePerMinute.HasValue && assessment.RespiratoryRatePerMinute.Value >= 28)
        {
            AddScore("Critical Care Physician", 4, $"Respiratory rate {assessment.RespiratoryRatePerMinute.Value}/min");
        }

        if (assessment.BloodSugarMgDl.HasValue && assessment.BloodSugarMgDl.Value >= 300m)
        {
            AddScore("Endocrinologist", 5, $"Blood sugar {assessment.BloodSugarMgDl.Value:0.#} mg/dL");
            AddScore("Critical Care Physician", 3, "Very high blood sugar");
        }
        else if (assessment.BloodSugarMgDl.HasValue && assessment.BloodSugarMgDl.Value >= 200m)
        {
            AddScore("Endocrinologist", 4, $"Blood sugar {assessment.BloodSugarMgDl.Value:0.#} mg/dL");
        }

        if (HasAny("chest pain", "palpitations", "leg swelling"))
        {
            AddScore("Cardiologist", 5, "Cardiac symptoms selected");
        }

        if (HasAny("shortness of breath"))
        {
            AddScore("Cardiologist", 3, "Shortness of breath");

            if (assessment.OxygenSaturationPercent.HasValue && assessment.OxygenSaturationPercent.Value <= 94)
            {
                AddScore("Critical Care Physician", 4, "Breathing concern with reduced oxygen");
            }
        }

        if (HasAny("headache", "dizziness", "numbness", "weakness", "fainting", "seizure"))
        {
            AddScore("Neurologist", 5, "Neurological symptoms selected");
        }

        if (HasAny("rash", "itching", "skin lesion", "acne", "hair loss"))
        {
            AddScore("Dermatologist", 5, "Skin-related symptoms selected");
        }

        if (HasAny("anxiety", "depression", "insomnia", "panic"))
        {
            AddScore("Psychiatrist", 5, "Mental health symptoms selected");
        }

        if (HasAny("abdominal pain", "nausea", "vomiting", "diarrhea", "constipation", "heartburn", "bloating", "jaundice"))
        {
            AddScore("Gastroenterologist", 5, "Digestive symptoms selected");
        }

        if (HasAny("excessive thirst", "frequent urination", "weight loss", "weight gain", "fatigue"))
        {
            AddScore("Endocrinologist", 4, "Metabolic symptoms selected");
        }

        if (HasAny("joint pain", "back pain", "neck pain", "knee pain", "fracture", "stiffness"))
        {
            AddScore("Orthopedic Surgeon", 5, "Musculoskeletal symptoms selected");
        }

        if (HasAny("fever", "cough", "sore throat"))
        {
            AddScore("Infectious Disease Specialist", 4, "Possible infection symptoms selected");
        }

        if (HasAny("sneezing", "wheezing", "runny nose", "hives"))
        {
            AddScore("Allergist", 5, "Allergy-related symptoms selected");
        }

        if (HasAny("burning urination", "blood in urine", "flank pain"))
        {
            AddScore("Nephrologist", 5, "Urinary or kidney-related symptoms selected");
        }

        if (HasAny("fever", "cough", "shortness of breath") &&
            assessment.TemperatureCelsius.HasValue &&
            assessment.TemperatureCelsius.Value >= 38m &&
            assessment.OxygenSaturationPercent.HasValue &&
            assessment.OxygenSaturationPercent.Value <= 94)
        {
            AddScore("Critical Care Physician", 5, "Respiratory distress pattern");
        }

        if (scores.Count == 0)
        {
            AddScore(DefaultSpecialization, 1, "General clinical triage");
        }

        var priorityOrder = new[]
        {
            "Critical Care Physician",
            "Cardiologist",
            "Neurologist",
            "Infectious Disease Specialist",
            "Endocrinologist",
            "Gastroenterologist",
            "Orthopedic Surgeon",
            "Dermatologist",
            "Psychiatrist",
            "Allergist",
            "Nephrologist",
            "Pediatrician",
            DefaultSpecialization
        };

        var selectedSpecialization = scores
            .OrderByDescending(x => x.Value)
            .ThenBy(x =>
            {
                var index = Array.FindIndex(priorityOrder, item => item.Equals(x.Key, StringComparison.OrdinalIgnoreCase));
                return index < 0 ? int.MaxValue : index;
            })
            .Select(x => x.Key)
            .First();

        var resolvedSpecialization = ResolveSpecialization(selectedSpecialization, availableSpecializations);

        if (string.IsNullOrWhiteSpace(resolvedSpecialization))
        {
            resolvedSpecialization = availableSpecializations.FirstOrDefault(
                                       x => Normalize(x) == Normalize(DefaultSpecialization))
                                   ?? selectedSpecialization;
        }

        var matchedSignals = signals.TryGetValue(selectedSpecialization, out var matched)
            ? matched.Take(5).ToList()
            : new List<string>();

        var summary = BuildAssessmentSummary(assessment, symptoms, resolvedSpecialization, matchedSignals);

        return new AssessmentRecommendation
        {
            Specialization = resolvedSpecialization,
            Signals = matchedSignals,
            AssessmentSummary = summary
        };
    }

    private static string BuildAssessmentSummary(
        DoctorRecommendationAssessmentDto assessment,
        IReadOnlyCollection<string> symptoms,
        string specialization,
        IReadOnlyCollection<string> matchedSignals)
    {
        var vitals = new List<string>();

        if (assessment.SystolicBloodPressure.HasValue || assessment.DiastolicBloodPressure.HasValue)
        {
            vitals.Add($"BP {assessment.SystolicBloodPressure?.ToString() ?? "-"} / {assessment.DiastolicBloodPressure?.ToString() ?? "-"}");
        }

        if (assessment.TemperatureCelsius.HasValue)
            vitals.Add($"Temp {assessment.TemperatureCelsius.Value:0.#} C");

        if (assessment.HeartRateBpm.HasValue)
            vitals.Add($"HR {assessment.HeartRateBpm.Value} bpm");

        if (assessment.OxygenSaturationPercent.HasValue)
            vitals.Add($"SpO2 {assessment.OxygenSaturationPercent.Value}%");

        if (assessment.BloodSugarMgDl.HasValue)
            vitals.Add($"Sugar {assessment.BloodSugarMgDl.Value:0.#} mg/dL");

        var symptomSummary = symptoms.Count > 0
            ? string.Join(", ", symptoms.Take(4))
            : "no symptom checklist selected";

        var signalSummary = matchedSignals.Count > 0
            ? string.Join("; ", matchedSignals)
            : "general triage indicators";

        return $"Based on the selected assessment ({symptomSummary}{(vitals.Count > 0 ? $"; {string.Join(", ", vitals)}" : string.Empty)}), the best next specialty appears to be {specialization}. Key signals: {signalSummary}.";
    }

    private IReadOnlyCollection<string> GetAvailableSpecializations()
    {
        var specializationModels = applicationDbContext.Specializations
            .AsNoTracking()
            .Select(x => x.Title)
            .ToList();

        var specializationsDatasets = GetDatasetRecords()
            .Select(x => x.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return specializationModels
            .Concat(specializationsDatasets)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private List<DoctorProfileDto> GetDoctorsFromDatabase(string specialization, string? city, int limit)
    {
        var doctorRoleId = applicationDbContext.Roles
            .AsNoTracking()
            .Where(x => x.Id.ToString() == Constants.Roles.Doctor.Id)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (doctorRoleId == Guid.Empty)
            return new List<DoctorProfileDto>();

        var normalizedSpecialization = specialization.Trim().ToLowerInvariant();

        var doctorsQuery = applicationDbContext.Users
            .Where(x =>
                x.RoleId == doctorRoleId &&
                x.DoctorSpecializations.Any(ds =>
                    ds.Specialization != null && ds.Specialization.Title.ToLower() == normalizedSpecialization))
            .Include(x => x.Role)
            .Include(x => x.DoctorProfile)
            .Include(x => x.Schedules)
            .Include(x => x.DoctorSpecializations)
                .ThenInclude(x => x.Specialization)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(city))
        {
            var normalizedCity = city.Trim().ToLowerInvariant();
            doctorsQuery = doctorsQuery.Where(x => x.Address != null && x.Address.ToLower().Contains(normalizedCity));
        }

        return doctorsQuery
            .OrderByDescending(x => x.DoctorReviews.Any() ? x.DoctorReviews.Average(r => r.Rating) : 0)
            .ThenBy(x => x.Name)
            .Take(limit)
            .Select(x => x.ToDoctorProfileDto())
            .ToList();
    }

    private List<DoctorDirectoryDto> GetDatasetFallback(string specialization, string? city, int limit)
    {
        var normalizedSpecialization = Normalize(specialization);
        var normalizedCity = city?.Trim().ToLowerInvariant();

        var matches = GetDatasetRecords()
            .Where(x =>
                Normalize(x.Category) == normalizedSpecialization ||
                Normalize(x.Category).Contains(normalizedSpecialization) ||
                normalizedSpecialization.Contains(Normalize(x.Category)))
            .Where(x => string.IsNullOrWhiteSpace(normalizedCity) || x.City.ToLower().Contains(normalizedCity))
            .OrderByDescending(x => x.Rating)
            .Take(limit)
            .Select(x => new DoctorDirectoryDto
            {
                Name = x.Name,
                Category = x.Category,
                Address = x.Address,
                City = x.City,
                Rating = x.Rating
            })
            .ToList();

        return matches;
    }

    private List<DoctorDirectoryRecordDto> GetDatasetRecords()
    {
        if (_dataset.Value.Count != 0) return _dataset.Value;

        var datasetPath = ResolveDatasetPath();

        if (!File.Exists(datasetPath)) return _dataset.Value;

        var lines = File.ReadAllLines(datasetPath);

        if (lines.Length <= 1) return _dataset.Value;

        var header = lines[0].ParseCsvLine();
        var categoryIndex = Array.FindIndex(header, x => string.Equals(x, "Category", StringComparison.OrdinalIgnoreCase));
        var nameIndex = Array.FindIndex(header, x => string.Equals(x, "Name", StringComparison.OrdinalIgnoreCase));
        var addressIndex = Array.FindIndex(header, x => string.Equals(x, "Address", StringComparison.OrdinalIgnoreCase));
        var cityIndex = Array.FindIndex(header, x => string.Equals(x, "City", StringComparison.OrdinalIgnoreCase));
        var ratingIndex = Array.FindIndex(header, x => string.Equals(x, "Rating", StringComparison.OrdinalIgnoreCase));

        if (categoryIndex < 0 || nameIndex < 0 || addressIndex < 0 || cityIndex < 0 || ratingIndex < 0)
            return _dataset.Value;

        for (var i = 1; i < lines.Length; i++)
        {
            var values = lines[i].ParseCsvLine();

            if (values.Length <= ratingIndex) continue;

            var ratingValue = ParseDecimal(values[ratingIndex]);

            var record = new DoctorDirectoryRecordDto
            {
                Category = values[categoryIndex],
                Name = values[nameIndex],
                Address = values[addressIndex],
                City = values[cityIndex],
                Rating = ratingValue
            };

            _dataset.Value.Add(record);
        }

        return _dataset.Value;
    }

    private string ResolveDatasetPath()
    {
        var webRoot = webHostEnvironment.WebRootPath;

        return !string.IsNullOrWhiteSpace(webRoot)
            ? Path.Combine(webRoot, DatasetRelativePath)
            : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", DatasetRelativePath);
    }

    private static decimal ParseDecimal(string input)
    {
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : 0m;
    }

    private static string ResolveSpecialization(string query, IReadOnlyCollection<string> options)
    {
        var normalizedQuery = Normalize(query);

        var exactMatch = options.FirstOrDefault(x => Normalize(x) == normalizedQuery);

        if (!string.IsNullOrWhiteSpace(exactMatch)) return exactMatch;

        var containsMatch = options.FirstOrDefault(x =>
            Normalize(x).Contains(normalizedQuery) || normalizedQuery.Contains(Normalize(x)));
        if (!string.IsNullOrWhiteSpace(containsMatch)) return containsMatch;

        var queryTokens = Tokenize(query);

        var bestMatch = options
            .Select(x => new
            {
                Title = x,
                Score = Tokenize(x).Intersect(queryTokens).Count()
            })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (bestMatch != null && bestMatch.Score > 0)
        {
            return bestMatch.Title;
        }

        return query.Trim();
    }

    private static HashSet<string> Tokenize(string input)
    {
        var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var current = new StringBuilder();

        foreach (var character in input)
        {
            if (char.IsLetterOrDigit(character))
            {
                current.Append(char.ToLowerInvariant(character));
                continue;
            }

            if (current.Length <= 0) continue;

            tokens.Add(current.ToString());
            current.Clear();
        }

        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
        }

        return tokens;
    }

    private static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var normalized = new StringBuilder();

        foreach (var character in input.Where(char.IsLetterOrDigit))
        {
            normalized.Append(char.ToLowerInvariant(character));
        }

        return normalized.ToString();
    }

    private sealed class AssessmentRecommendation
    {
        public string Specialization { get; init; } = string.Empty;

        public List<string> Signals { get; init; } = new();

        public string AssessmentSummary { get; init; } = string.Empty;
    }
}
