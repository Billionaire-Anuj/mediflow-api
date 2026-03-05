using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class DoctorReviewConfigurations : IEntityTypeConfiguration<DoctorReview>
{
    public void Configure(EntityTypeBuilder<DoctorReview> builder)
    {
        builder
            .Property(x => x.AppointmentId)
            .IsRequired();

        builder
            .Property(x => x.DoctorId)
            .IsRequired();

        builder
            .Property(x => x.PatientId)
            .IsRequired();

        builder
            .Property(x => x.Rating)
            .IsRequired();

        builder
            .Property(x => x.Review)
            .HasMaxLength(1024)
            .IsRequired(false);

        builder
            .HasOne(x => x.Appointment)
            .WithOne(x => x.Review)
            .HasForeignKey<DoctorReview>(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Doctor)
            .WithMany(x => x.DoctorReviews)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Patient)
            .WithMany(x => x.PatientReviews)
            .HasForeignKey(x => x.PatientId)
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
            .HasIndex(x => x.AppointmentId)
            .IsUnique();

        builder
            .HasIndex(x => x.DoctorId);

        builder
            .HasIndex(x => x.PatientId);
    }
}
