using Mediflow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;
using Mediflow.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class DiagnosticTypeSeeder(ILogger<DiagnosticTypeSeeder> logger, IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public int Order => 40;

    public void Seed()
    {
        var specifications = new List<(string Title, string Description)>
        {
            ("Hematology", "Blood-related diagnostic investigations and analyses."),
            ("Biochemistry", "Chemical analysis of bodily fluids and tissues."),
            ("Microbiology", "Tests for infectious agents such as bacteria and viruses."),
            ("Immunology", "Assessment of immune system responses and disorders."),
            ("Radiology", "Imaging-based diagnostic evaluations."),
            ("Cardiology", "Heart and cardiovascular diagnostic tests."),
            ("Neurology", "Diagnostic tests for brain and nervous system conditions."),
            ("Endocrinology", "Hormonal and endocrine system diagnostics."),
            ("Oncology", "Diagnostics related to cancer detection and monitoring."),
            ("Pathology", "Tissue and specimen examination for disease diagnosis.")
        };

        var existing = applicationDbContext.DiagnosticTypes.ToList();
        var existingSet = new HashSet<string>(existing.Select(x => x.Title), StringComparer.OrdinalIgnoreCase);

        var tenantAdministratorRole = applicationDbContext.Roles
            .AsNoTracking()
            .First(x => x.Id.ToString() == Constants.Roles.TenantAdministrator.Id);

        var tenantAdministratorUser = applicationDbContext.Users
            .AsNoTracking()
            .First(x => x.RoleId == tenantAdministratorRole.Id);
        
        foreach (var specification in specifications)
        {
            var diagnosticType = existing.FirstOrDefault(x => string.Equals(x.Title, specification.Title, StringComparison.OrdinalIgnoreCase));

            if (diagnosticType != null)
            {
                diagnosticType.Update(specification.Title, specification.Description);
                diagnosticType.LastModifiedBy = tenantAdministratorUser.Id;
                applicationDbContext.DiagnosticTypes.Update(diagnosticType);
                continue;
            }

            if (!existingSet.Contains(specification.Title))
            {
                applicationDbContext.DiagnosticTypes.Add(new DiagnosticType(specification.Title, specification.Description)
                {
                    CreatedBy = tenantAdministratorUser.Id
                });
            }
        }

        applicationDbContext.SaveChanges();
        logger.LogInformation("Diagnostic types initialization successfully completed.");
    }
}
