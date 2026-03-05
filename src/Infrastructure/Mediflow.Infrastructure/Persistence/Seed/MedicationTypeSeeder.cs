using Mediflow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;
using Mediflow.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class MedicationTypeSeeder(ILogger<MedicationTypeSeeder> logger, IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public int Order => 60;

    public void Seed()
    {
        var specifications = new List<(string Title, string Description)>
        {
            ("Antibiotic", "Medications used to treat bacterial infections."),
            ("Analgesic", "Medications used to relieve pain."),
            ("Antipyretic", "Medications used to reduce fever."),
            ("Antihistamine", "Medications used to treat allergies."),
            ("Antidiabetic", "Medications used to control blood sugar levels."),
            ("Antihypertensive", "Medications used to manage high blood pressure."),
            ("Antacid", "Medications used to neutralize stomach acid."),
            ("Antidepressant", "Medications used to treat depression and mood disorders."),
            ("Corticosteroid", "Medications used to reduce inflammation."),
            ("Antiviral", "Medications used to treat viral infections.")
        };

        var existing = applicationDbContext.MedicationTypes.ToList();
        var existingSet = new HashSet<string>(existing.Select(x => x.Title), StringComparer.OrdinalIgnoreCase);

        var tenantAdministratorRole = applicationDbContext.Roles
            .AsNoTracking()
            .First(x => x.Id.ToString() == Constants.Roles.TenantAdministrator.Id);

        var tenantAdministratorUser = applicationDbContext.Users
            .AsNoTracking()
            .First(x => x.RoleId == tenantAdministratorRole.Id);

        foreach (var specification in specifications)
        {
            var medicationType = existing.FirstOrDefault(x => string.Equals(x.Title, specification.Title, StringComparison.OrdinalIgnoreCase));

            if (medicationType != null)
            {
                medicationType.Update(specification.Title, specification.Description);
                applicationDbContext.MedicationTypes.Update(medicationType);
                continue;
            }

            if (!existingSet.Contains(specification.Title))
            {
                applicationDbContext.MedicationTypes.Add(new MedicationType(specification.Title, specification.Description){
                    CreatedBy = tenantAdministratorUser.Id
                });
            }
        }

        applicationDbContext.SaveChanges();
        logger.LogInformation("Medication types initialization successfully completed.");
    }
}
