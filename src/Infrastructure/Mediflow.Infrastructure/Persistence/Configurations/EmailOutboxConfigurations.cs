using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class EmailOutboxConfiguration : IEntityTypeConfiguration<EmailOutbox>
{
    public void Configure(EntityTypeBuilder<EmailOutbox> builder)
    {
        builder
            .Property(x => x.ToEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(x => x.Subject)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.Process)
            .HasConversion<string>()
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(x => x.PayloadJson)
            .HasMaxLength(1024)
            .HasColumnType("jsonb")
            .HasDefaultValue("{}")
            .IsRequired();

        builder
            .Property(x => x.AttemptCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder
            .Property(x => x.NextAttemptDate)
            .IsRequired();

        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder
            .Property(x => x.ScheduledDate)
            .IsRequired();

        builder
            .Property(x => x.SentDate)
            .IsRequired(false);

        builder
            .Property(x => x.LastError)
            .HasMaxLength(512);

        builder
            .HasIndex(x => x.Status);

        builder
            .HasIndex(x => x.NextAttemptDate);

        builder
            .HasIndex(x => new
            {
                x.ToEmail,
                x.Process,
                x.Subject
            });
    }
}
