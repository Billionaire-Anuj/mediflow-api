using Mediflow.Domain.Entities.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations.Audits;

public class AuditLogHistoryConfigurations : IEntityTypeConfiguration<AuditLogHistory>
{
    public void Configure(EntityTypeBuilder<AuditLogHistory> builder)
    {
        builder
            .Property(x => x.AuditLogId)
            .IsRequired();

        builder
            .Property(x => x.FieldName)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(x => x.FieldDataType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(x => x.OldValue)
            .HasMaxLength(512)
            .IsRequired(false);

        builder
            .Property(x => x.NewValue)
            .HasMaxLength(512)
            .IsRequired(false);

        builder
            .Property(x => x.Remarks)
            .HasMaxLength(512)
            .IsRequired(false);

        builder
            .HasOne(x => x.AuditLog)
            .WithMany(x => x.AuditLogHistories)
            .HasForeignKey(x => x.AuditLogId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.AuditLogId);
    }
}