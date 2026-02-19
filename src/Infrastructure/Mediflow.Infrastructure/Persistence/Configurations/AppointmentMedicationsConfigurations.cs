using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentMedicationsConfigurations : IEntityTypeConfiguration<AppointmentMedications>
{
    public void Configure(EntityTypeBuilder<AppointmentMedications> builder)
    {
        builder
            .Property(x => x.AppointmentId)
            .IsRequired();

        builder
            .Property(x => x.PharmacistId)
            .IsRequired(false);

        builder
            .Property(x => x.Notes)
            .HasMaxLength(1024)
            .IsRequired();

        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder
            .Property(x => x.CompletedDate)
            .IsRequired(false);

        builder
            .HasOne(x => x.Appointment)
            .WithMany()
            .HasForeignKey(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Pharmacist)
            .WithMany(x => x.AppointmentMedications)
            .HasForeignKey(x => x.PharmacistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.Drugs)
            .WithOne(x => x.AppointmentMedications)
            .HasForeignKey(x => x.AppointmentMedicationsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.AppointmentId);

        builder
            .HasIndex(x => x.PharmacistId);

        builder
            .HasIndex(x => x.Status);
    }
}
