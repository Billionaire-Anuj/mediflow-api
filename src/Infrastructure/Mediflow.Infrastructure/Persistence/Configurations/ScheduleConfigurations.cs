using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class ScheduleConfigurations : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder
            .Property(x => x.DoctorId)
            .IsRequired();

        builder
            .Property(x => x.DayOfWeek)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder
            .Property(x => x.StartTime)
            .IsRequired();

        builder
            .Property(x => x.EndTime)
            .IsRequired();

        builder
            .Property(x => x.SlotDurationInMinutes)
            .HasDefaultValue(30)
            .IsRequired();

        builder
            .Property(x => x.IsAvailable)
            .HasDefaultValue(true)
            .IsRequired();

        builder
            .Property(x => x.ValidStartDate)
            .IsRequired();

        builder
            .Property(x => x.ValidEndDate)
            .IsRequired();

        builder
            .Property(x => x.Notes)
            .HasMaxLength(512)
            .IsRequired(false);

        builder
            .HasMany(x => x.Timeslots)
            .WithOne(x => x.Schedule)
            .HasForeignKey(x => x.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Doctor)
            .WithMany(x => x.Schedules)
            .HasForeignKey(x => x.DoctorId)
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
            .HasIndex(x => x.DoctorId);

        builder
            .HasIndex(x => new
            {
                x.DoctorId,
                x.DayOfWeek,
                x.StartTime,
                x.EndTime
            });
    }
}
