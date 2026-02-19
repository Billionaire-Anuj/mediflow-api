using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class DiagnosticTestConfigurations : IEntityTypeConfiguration<DiagnosticTest>
{
    public void Configure(EntityTypeBuilder<DiagnosticTest> builder)
    {
        builder
            .Property(x => x.DiagnosticTypeId)
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
            .Property(x => x.Specimen)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .HasOne(x => x.DiagnosticType)
            .WithMany(x => x.DiagnosticTests)
            .HasForeignKey(x => x.DiagnosticTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.AppointmentDiagnosticTests)
            .WithOne(x => x.DiagnosticTest)
            .HasForeignKey(x => x.DiagnosticTestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.DiagnosticTypeId);

        builder
            .HasIndex(x => x.Title);
    }
}
