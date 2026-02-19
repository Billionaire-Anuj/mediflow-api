using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class MedicationTypeConfigurations : IEntityTypeConfiguration<MedicationType>
{
    public void Configure(EntityTypeBuilder<MedicationType> builder)
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
            .HasMany(x => x.Medicines)
            .WithOne(x => x.MedicationType)
            .HasForeignKey(x => x.MedicationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.Title)
            .IsUnique();
    }
}
