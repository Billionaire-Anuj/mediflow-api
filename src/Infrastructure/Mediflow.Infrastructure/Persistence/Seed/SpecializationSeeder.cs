using Mediflow.Helper;
using Mediflow.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;
using Mediflow.Domain.Common;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class SpecializationSeeder(
    ILogger<SpecializationSeeder> logger,
    IWebHostEnvironment webHostEnvironment,
    IApplicationDbContext applicationDbContext) : IDataSeeder
{
    private const string DatasetRelativePath = "data/recommendations/doctors.csv";

    public int Order => 50;

    public void Seed()
    {
        var datasetPath = ResolveDatasetPath();

        if (!File.Exists(datasetPath))
        {
            logger.LogWarning("Specialization seed dataset not found at {DatasetPath}.", datasetPath);
            return;
        }

        var lines = File.ReadAllLines(datasetPath);

        if (lines.Length <= 1) return;

        var header = lines[0].ParseCsvLine();

        var categoryIndex = Array.FindIndex(header, x => string.Equals(x, "Category", StringComparison.OrdinalIgnoreCase));

        if (categoryIndex < 0) return;

        var duplicateSpecializations = applicationDbContext.Specializations
            .AsNoTracking()
            .Select(x => x.Title)
            .ToList();

        var duplicateSpecializationSet = new HashSet<string>(duplicateSpecializations, StringComparer.OrdinalIgnoreCase);

        var specializationModels = new List<Specialization>();

        var tenantAdministratorRole = applicationDbContext.Roles
            .AsNoTracking()
            .First(x => x.Id.ToString() == Constants.Roles.TenantAdministrator.Id);

        var tenantAdministratorUser = applicationDbContext.Users
            .AsNoTracking()
            .First(x => x.RoleId == tenantAdministratorRole.Id);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = lines[i].ParseCsvLine();

            if (values.Length <= categoryIndex) continue;

            var title = values[categoryIndex].Trim();

            if (string.IsNullOrWhiteSpace(title)) continue;

            if (duplicateSpecializationSet.Contains(title)) continue;

            specializationModels.Add(new Specialization(title, "Seeded from doctor recommendation dataset."));

            duplicateSpecializationSet.Add(title);
        }

        if (specializationModels.Count == 0) return;

        specializationModels.ForEach(x => x.CreatedBy = tenantAdministratorUser.Id);

        applicationDbContext.Specializations.AddRange(specializationModels);

        applicationDbContext.SaveChanges();

        logger.LogInformation("Specialization initialization successfully completed.");
    }

    private string ResolveDatasetPath()
    {
        var webRoot = webHostEnvironment.WebRootPath;

        return !string.IsNullOrWhiteSpace(webRoot)
            ? Path.Combine(webRoot, DatasetRelativePath)
            : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", DatasetRelativePath);
    }
}
