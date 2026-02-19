using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class DoctorSpecializationConfigurations : IEntityTypeConfiguration<DoctorSpecialization>
{
    public void Configure(EntityTypeBuilder<DoctorSpecialization> builder)
    {
        builder
            .Property(x => x.DoctorId)
            .IsRequired();

        builder
            .Property(x => x.SpecializationId)
            .IsRequired();

        builder
            .HasOne(x => x.Doctor)
            .WithMany(x => x.DoctorSpecializations)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Specialization)
            .WithMany(x => x.DoctorSpecializations)
            .HasForeignKey(x => x.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new
            {
                x.DoctorId,
                x.SpecializationId
            })
            .IsUnique();
    }
}
