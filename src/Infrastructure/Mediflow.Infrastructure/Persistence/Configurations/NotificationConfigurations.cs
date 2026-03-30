using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfigurations : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder
            .Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(x => x.Title)
            .HasMaxLength(160)
            .IsRequired();

        builder
            .Property(x => x.Message)
            .HasMaxLength(1024)
            .IsRequired();

        builder
            .Property(x => x.ActionUrl)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.NotificationKey)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.IsRead)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .Property(x => x.ReadAt)
            .IsRequired(false);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.UserId);

        builder
            .HasIndex(x => x.IsRead);

        builder
            .HasIndex(x => x.Type);

        builder
            .HasIndex(x => new { x.UserId, x.NotificationKey })
            .IsUnique();
    }
}
