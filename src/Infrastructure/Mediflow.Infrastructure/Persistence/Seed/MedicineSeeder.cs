using Mediflow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Seed;
using Mediflow.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Persistence.Seed;

public class MedicineSeeder(ILogger<MedicineSeeder> logger, IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public int Order => 70;

    public void Seed()
    {
        var medicationTypes = applicationDbContext.MedicationTypes.ToList();
        var typeLookup = medicationTypes.ToDictionary(x => x.Title, StringComparer.OrdinalIgnoreCase);

        var specifications = new List<(string Title, string Description, string Format, string MedicationType)>
        {
            ("Amoxicillin", "Broad-spectrum antibiotic for bacterial infections.", "Capsule", "Antibiotic"),
            ("Azithromycin", "Macrolide antibiotic for respiratory infections.", "Tablet", "Antibiotic"),
            ("Paracetamol", "Pain reliever and fever reducer.", "Tablet", "Antipyretic"),
            ("Ibuprofen", "Nonsteroidal anti-inflammatory drug for pain.", "Tablet", "Analgesic"),
            ("Cetirizine", "Allergy relief antihistamine.", "Tablet", "Antihistamine"),
            ("Metformin", "Blood glucose control for type 2 diabetes.", "Tablet", "Antidiabetic"),
            ("Amlodipine", "Calcium channel blocker for hypertension.", "Tablet", "Antihypertensive"),
            ("Omeprazole", "Reduces stomach acid production.", "Capsule", "Antacid"),
            ("Prednisone", "Steroid for inflammation reduction.", "Tablet", "Corticosteroid"),
            ("Oseltamivir", "Antiviral medication for influenza.", "Capsule", "Antiviral")
        };

        var existingMedicines = applicationDbContext.Medicines.ToList();

        var tenantAdministratorRole = applicationDbContext.Roles
            .AsNoTracking()
            .First(x => x.Id.ToString() == Constants.Roles.TenantAdministrator.Id);

        var tenantAdministratorUser = applicationDbContext.Users
            .AsNoTracking()
            .First(x => x.RoleId == tenantAdministratorRole.Id);

        foreach (var specification in specifications)
        {
            if (!typeLookup.TryGetValue(specification.MedicationType, out var medicationType))
            {
                medicationType = new MedicationType(specification.MedicationType, $"{specification.MedicationType} medication category.");
                applicationDbContext.MedicationTypes.Add(medicationType);
                applicationDbContext.SaveChanges();
                typeLookup[specification.MedicationType] = medicationType;
            }

            var medicine = existingMedicines
                .FirstOrDefault(x => string.Equals(x.Title, specification.Title, StringComparison.OrdinalIgnoreCase));

            if (medicine != null)
            {
                medicine.Update(medicationType.Id, specification.Title, specification.Description, specification.Format);
                medicine.LastModifiedBy = tenantAdministratorUser.Id;
                applicationDbContext.Medicines.Update(medicine);
                continue;
            }

            applicationDbContext.Medicines.Add(new Medicine(
                medicationType.Id,
                specification.Title,
                specification.Description,
                specification.Format){
                CreatedBy = tenantAdministratorUser.Id
            });
        }

        applicationDbContext.SaveChanges();
        logger.LogInformation("Medicines initialization successfully completed.");
    }
}
