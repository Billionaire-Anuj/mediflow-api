using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class DiagnosticTypeConfigurations : IEntityTypeConfiguration<DiagnosticType>
{
    public void Configure(EntityTypeBuilder<DiagnosticType> builder)
    {
        builder
            .Property(x => x.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder
            .Property(x => x.Description)
            .HasMaxLength(512)
            .IsRequired();

        builder
            .HasMany(x => x.DiagnosticTests)
            .WithOne(x => x.DiagnosticType)
            .HasForeignKey(x => x.DiagnosticTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.Title)
            .IsUnique();
    }
}
