using Mediflow.Helper;
using System.Globalization;
using Mediflow.Domain.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Doctors;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Recommendations;

namespace Mediflow.Infrastructure.Implementation.Services;

public class DoctorRecommendationService(
    IWebHostEnvironment webHostEnvironment,
    IApplicationDbContext applicationDbContext) : IDoctorRecommendationService
{
    private const string DatasetRelativePath = "data/recommendations/doctors.csv";

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
            RecommendedSpecialization = recommendedSpecialization,
            Doctors = doctors
        };

        if (doctors.Count == 0)
        {
            result.DatasetFallback = GetDatasetFallback(recommendedSpecialization, city, limit);
        }

        return result;
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
            .OrderBy(x => x.Name)
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

        if (!string.IsNullOrWhiteSpace(webRoot))
        {
            return Path.Combine(webRoot, DatasetRelativePath);
        }

        var fallback = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", DatasetRelativePath);

        return fallback;
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
        var current = new System.Text.StringBuilder();

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

        var normalized = new System.Text.StringBuilder();

        foreach (var character in input.Where(char.IsLetterOrDigit))
        {
            normalized.Append(char.ToLowerInvariant(character));
        }

        return normalized.ToString();
    }
}
