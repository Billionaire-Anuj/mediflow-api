using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class TimeslotConfigurations : IEntityTypeConfiguration<Timeslot>
{
    public void Configure(EntityTypeBuilder<Timeslot> builder)
    {
        builder
            .Property(x => x.ScheduleId)
            .IsRequired();

        builder
            .Property(x => x.Date)
            .IsRequired();

        builder
            .Property(x => x.StartTime)
            .IsRequired();

        builder
            .Property(x => x.EndTime)
            .IsRequired();

        builder
            .Property(x => x.IsBooked)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .HasOne(x => x.Schedule)
            .WithMany()
            .HasForeignKey(x => x.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.ScheduleId);

        builder
            .HasIndex(x => new
            {
                x.ScheduleId,
                x.Date,
                x.StartTime,
                x.EndTime
            })
            .IsUnique();
    }
}
