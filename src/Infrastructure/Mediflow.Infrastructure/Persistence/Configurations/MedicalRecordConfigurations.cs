using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class MedicalRecordConfigurations : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder
            .Property(x => x.AppointmentId)
            .IsRequired();

        builder
            .Property(x => x.Diagnosis)
            .HasMaxLength(1024)
            .IsRequired();

        builder
            .Property(x => x.Treatment)
            .HasMaxLength(1024)
            .IsRequired();

        builder
            .Property(x => x.Prescriptions)
            .HasMaxLength(1024)
            .IsRequired();

        builder
            .Property(x => x.Notes)
            .HasMaxLength(2048)
            .IsRequired(false);

        builder
            .HasOne(x => x.Appointment)
            .WithOne(x => x.MedicalRecord)
            .HasForeignKey<MedicalRecord>(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.AppointmentId)
            .IsUnique();
    }
}
