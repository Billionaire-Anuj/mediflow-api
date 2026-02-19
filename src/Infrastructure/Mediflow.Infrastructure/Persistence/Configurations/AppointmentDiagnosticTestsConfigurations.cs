using System.Text.Json;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentDiagnosticTestsConfigurations : IEntityTypeConfiguration<AppointmentDiagnosticTests>
{
    public void Configure(EntityTypeBuilder<AppointmentDiagnosticTests> builder)
    {
        builder
            .Property(x => x.AppointmentDiagnosticsId)
            .IsRequired();

        builder
            .Property(x => x.DiagnosticTestId)
            .IsRequired();

        builder
            .Property(x => x.DiagnosticReport)
            .HasConversion(
                v => JsonSerializer.Serialize(v, AssetConfigurations.JsonOptions),
                v => JsonSerializer.Deserialize<Asset>(v, AssetConfigurations.JsonOptions)!)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder
            .HasOne(x => x.AppointmentDiagnostics)
            .WithMany(x => x.DiagnosticTests)
            .HasForeignKey(x => x.AppointmentDiagnosticsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.DiagnosticTest)
            .WithMany(x => x.AppointmentDiagnosticTests)
            .HasForeignKey(x => x.DiagnosticTestId)
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
                x.AppointmentDiagnosticsId,
                x.DiagnosticTestId
            })
            .IsUnique();
    }
}
