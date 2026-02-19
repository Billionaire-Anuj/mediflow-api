using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder
            .Property(x => x.DoctorId)
            .IsRequired();

        builder
            .Property(x => x.PatientId)
            .IsRequired();

        builder
            .Property(x => x.BookedDate)
            .IsRequired();

        builder
            .Property(x => x.TimeslotId)
            .IsRequired();

        builder
            .Property(x => x.AppointmentTime)
            .IsRequired();

        builder
            .Property(x => x.CancelledDate)
            .IsRequired(false);

        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder
            .Property(x => x.Notes)
            .HasMaxLength(1024)
            .IsRequired(false);

        builder
            .Property(x => x.Symptoms)
            .HasMaxLength(1024)
            .IsRequired(false);

        builder
            .Property(x => x.Fee)
            .HasPrecision(18, 2)
            .IsRequired();

        builder
            .Property(x => x.IsPaidViaGateway)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .Property(x => x.IsPaidViaOfflineMedium)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .Property(x => x.CancellationReason)
            .HasMaxLength(512)
            .IsRequired(false);

        builder
            .HasOne(x => x.Doctor)
            .WithMany(x => x.DoctorAppointments)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Patient)
            .WithMany(x => x.PatientAppointments)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Timeslot)
            .WithOne(x => x.Appointment)
            .HasForeignKey<Appointment>(x => x.TimeslotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.MedicalRecord)
            .WithOne(x => x.Appointment)
            .HasForeignKey<MedicalRecord>(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.AppointmentDiagnostics)
            .WithOne(x => x.Appointment)
            .HasForeignKey(x => x.AppointmentId)
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
            .HasIndex(x => x.DoctorId);

        builder
            .HasIndex(x => x.PatientId);

        builder
            .HasIndex(x => x.Status);

        builder
            .HasIndex(x => x.BookedDate);

        builder
            .HasIndex(x => x.TimeslotId);
    }
}
