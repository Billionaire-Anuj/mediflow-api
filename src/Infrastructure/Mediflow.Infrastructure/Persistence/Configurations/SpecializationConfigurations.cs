using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class SpecializationConfigurations : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
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
            .HasMany(x => x.DoctorSpecializations)
            .WithOne(x => x.Specialization)
            .HasForeignKey(x => x.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.Title)
            .IsUnique();
    }
}
