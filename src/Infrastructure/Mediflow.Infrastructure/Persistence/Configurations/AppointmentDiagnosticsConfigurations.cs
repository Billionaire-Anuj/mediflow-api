using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentDiagnosticsConfigurations : IEntityTypeConfiguration<AppointmentDiagnostics>
{
    public void Configure(EntityTypeBuilder<AppointmentDiagnostics> builder)
    {
        builder
            .Property(x => x.AppointmentId)
            .IsRequired();

        builder
            .Property(x => x.LabTechnicianId)
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
            .WithMany(x => x.AppointmentDiagnostics)
            .HasForeignKey(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.LabTechnician)
            .WithMany(x => x.AppointmentDiagnostics)
            .HasForeignKey(x => x.LabTechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.DiagnosticTests)
            .WithOne(x => x.AppointmentDiagnostics)
            .HasForeignKey(x => x.AppointmentDiagnosticsId)
            .OnDelete(DeleteBehavior.Cascade);

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
            .HasIndex(x => x.AppointmentId);

        builder
            .HasIndex(x => x.LabTechnicianId);

        builder
            .HasIndex(x => x.Status);
    }
}
