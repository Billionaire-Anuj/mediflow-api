using Mediflow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;
using Mediflow.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class DiagnosticTestSeeder(ILogger<DiagnosticTestSeeder> logger, IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public int Order => 50;

    public void Seed()
    {
        var diagnosticTypes = applicationDbContext.DiagnosticTypes.ToList();
        var typeLookup = diagnosticTypes.ToDictionary(x => x.Title, StringComparer.OrdinalIgnoreCase);

        var specifications = new List<(string Title, string Description, string Specimen, string DiagnosticType)>
        {
            ("Complete Blood Count", "Measures red and white blood cells and platelets.", "Blood", "Hematology"),
            ("Lipid Profile", "Measures cholesterol and triglycerides.", "Blood", "Biochemistry"),
            ("HbA1c", "Average blood glucose over 2-3 months.", "Blood", "Endocrinology"),
            ("Thyroid Function Test", "Assesses thyroid hormone levels.", "Blood", "Endocrinology"),
            ("Chest X-Ray", "Imaging of chest organs.", "Imaging", "Radiology"),
            ("MRI Brain", "Detailed imaging of brain structures.", "Imaging", "Radiology"),
            ("Electrocardiogram", "Measures electrical activity of the heart.", "Electrocardiogram", "Cardiology"),
            ("Echocardiogram", "Ultrasound imaging of the heart.", "Imaging", "Cardiology"),
            ("Urine Culture", "Detects bacterial infection in urine.", "Urine", "Microbiology"),
            ("Histopathology", "Microscopic examination of tissue.", "Tissue", "Pathology")
        };

        var existingTests = applicationDbContext.DiagnosticTests.ToList();

        var tenantAdministratorRole = applicationDbContext.Roles
            .AsNoTracking()
            .First(x => x.Id.ToString() == Constants.Roles.TenantAdministrator.Id);

        var tenantAdministratorUser = applicationDbContext.Users
            .AsNoTracking()
            .First(x => x.RoleId == tenantAdministratorRole.Id);
        
        foreach (var specification in specifications)
        {
            if (!typeLookup.TryGetValue(specification.DiagnosticType, out var diagnosticType))
            {
                diagnosticType = new DiagnosticType(specification.DiagnosticType, $"{specification.DiagnosticType} related diagnostic investigations.");
                applicationDbContext.DiagnosticTypes.Add(diagnosticType);
                applicationDbContext.SaveChanges();
                typeLookup[specification.DiagnosticType] = diagnosticType;
            }

            var diagnosticTest = existingTests
                .FirstOrDefault(x => string.Equals(x.Title, specification.Title, StringComparison.OrdinalIgnoreCase));

            if (diagnosticTest != null)
            {
                diagnosticTest.Update(diagnosticType.Id, specification.Title, specification.Description, specification.Specimen);
                diagnosticTest.LastModifiedBy = tenantAdministratorUser.Id;
                applicationDbContext.DiagnosticTests.Update(diagnosticTest);
                continue;
            }

            applicationDbContext.DiagnosticTests.Add(new DiagnosticTest(
                diagnosticType.Id,
                specification.Title,
                specification.Description,
                specification.Specimen)
            {
                CreatedBy = tenantAdministratorUser.Id
            });
        }

        applicationDbContext.SaveChanges();
        logger.LogInformation("Diagnostic tests initialization successfully completed.");
    }
}
