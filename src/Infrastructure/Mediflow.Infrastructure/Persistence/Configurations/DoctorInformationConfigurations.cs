using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class DoctorInformationConfigurations : IEntityTypeConfiguration<DoctorInformation>
{
    public void Configure(EntityTypeBuilder<DoctorInformation> builder)
    {
        builder
            .Property(x => x.DoctorId)
            .IsRequired();

        builder
            .Property(x => x.About)
            .HasMaxLength(2048)
            .IsRequired();

        builder
            .Property(x => x.LicenseNumber)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(x => x.EducationInformation)
            .HasMaxLength(2048)
            .IsRequired();

        builder
            .Property(x => x.ExperienceInformation)
            .HasMaxLength(2048)
            .IsRequired();

        builder
            .Property(x => x.ConsultationFee)
            .HasPrecision(18, 2)
            .IsRequired();

        builder
            .HasOne(x => x.Doctor)
            .WithOne(x => x.DoctorInformation)
            .HasForeignKey<DoctorInformation>(x => x.DoctorId)
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
            .HasIndex(x => x.DoctorId)
            .IsUnique();

        builder
            .HasIndex(x => x.LicenseNumber)
            .IsUnique();
    }
}
