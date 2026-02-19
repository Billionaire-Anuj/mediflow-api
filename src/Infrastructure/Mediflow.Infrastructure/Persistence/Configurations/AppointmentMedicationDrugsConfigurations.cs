using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentMedicationDrugsConfigurations : IEntityTypeConfiguration<AppointmentMedicationDrugs>
{
    public void Configure(EntityTypeBuilder<AppointmentMedicationDrugs> builder)
    {
        builder
            .Property(x => x.AppointmentMedicationsId)
            .IsRequired();

        builder
            .Property(x => x.MedicineId)
            .IsRequired();

        builder
            .Property(x => x.Dose)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(x => x.Frequency)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(x => x.Duration)
            .IsRequired();

        builder
            .Property(x => x.Instructions)
            .HasMaxLength(512)
            .IsRequired(false);

        builder
            .HasOne(x => x.AppointmentMedications)
            .WithMany(x => x.Drugs)
            .HasForeignKey(x => x.AppointmentMedicationsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Medicine)
            .WithMany(x => x.AppointmentMedicationDrugs)
            .HasForeignKey(x => x.MedicineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.CreatedUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.LastModifiedUser)
            .WithMany()
            .HasForeignKey(x => x.LastModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.DeletedUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new
            {
                x.AppointmentMedicationsId,
                x.MedicineId
            });
    }
}
