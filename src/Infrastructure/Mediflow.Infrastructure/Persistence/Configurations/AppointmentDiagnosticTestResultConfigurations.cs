using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentDiagnosticTestResultConfigurations : IEntityTypeConfiguration<AppointmentDiagnosticTestResult>
{
    public void Configure(EntityTypeBuilder<AppointmentDiagnosticTestResult> builder)
    {
        builder
            .Property(x => x.AppointmentDiagnosticTestId)
            .IsRequired();

        builder
            .Property(x => x.Value)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(x => x.Unit)
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(x => x.UpperRange)
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(x => x.LowerRange)
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(x => x.Interpretation)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .HasOne(x => x.AppointmentDiagnosticTests)
            .WithOne(x => x.AppointmentDiagnosticTestResult)
            .HasForeignKey<AppointmentDiagnosticTestResult>(x => x.AppointmentDiagnosticTestId)
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
            .HasIndex(x => x.AppointmentDiagnosticTestId)
            .IsUnique();
    }
}
