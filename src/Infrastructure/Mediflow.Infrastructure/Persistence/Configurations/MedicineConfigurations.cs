using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class MedicineConfigurations : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder
            .Property(x => x.MedicationTypeId)
            .IsRequired();

        builder
            .Property(x => x.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder
            .Property(x => x.Description)
            .HasMaxLength(512)
            .IsRequired();

        builder
            .Property(x => x.Format)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .HasOne(x => x.MedicationType)
            .WithMany(x => x.Medicines)
            .HasForeignKey(x => x.MedicationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.AppointmentMedicationDrugs)
            .WithOne(x => x.Medicine)
            .HasForeignKey(x => x.MedicineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.MedicationTypeId);

        builder
            .HasIndex(x => x.Title);
    }
}
