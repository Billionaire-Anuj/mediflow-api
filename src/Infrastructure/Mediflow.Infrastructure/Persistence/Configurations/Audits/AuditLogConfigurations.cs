using Mediflow.Domain.Entities.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations.Audits;

public class AuditLogConfigurations : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder
            .Property(x => x.EntityName)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(x => x.EntityId)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(x => x.ChangeType)
            .HasConversion<string>()
            .HasMaxLength(25)
            .IsRequired();

        builder
            .Property(x => x.Remarks)
            .HasMaxLength(256)
            .IsRequired(false);

        builder
            .Property(x => x.IsAutomation)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .Property(x => x.AuditedDate)
            .IsRequired();

        builder
            .Property(x => x.AuditorId)
            .IsRequired(false);

        builder
            .HasOne(x => x.Auditor)
            .WithMany(x => x.AuditLogs)
            .HasForeignKey(x => x.AuditorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.AuditLogHistories)
            .WithOne(x => x.AuditLog)
            .HasForeignKey(x => x.AuditLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new
            {
                x.EntityName,
                x.EntityId,
                x.AuditorId,
                x.AuditedDate
            });
    }
}