using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class UserLoginLogConfigurations : IEntityTypeConfiguration<UserLoginLog>
{
    public void Configure(EntityTypeBuilder<UserLoginLog> builder)
    {
        builder
            .Property(x => x.UserId)
            .IsRequired(false);

        builder
            .Property(x => x.EmailAddressOrUsername)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.EventType)
            .HasConversion<string>()
            .IsRequired();

        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder
            .Property(x => x.AccessToken)
            .HasMaxLength(2048)
            .IsRequired(false);

        builder
            .Property(x => x.IpAddress)
            .HasMaxLength(64)
            .IsRequired(false);

        builder
            .Property(x => x.UserAgent)
            .HasMaxLength(512)
            .IsRequired(false);

        builder
            .Property(x => x.IsActiveSession)
            .IsRequired();

        builder
            .Property(x => x.ActionDate)
            .IsRequired();

        builder
            .Property(x => x.LoggedOutDate)
            .IsRequired(false);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.UserLoginLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.UserId);

        builder
            .HasIndex(x => x.EmailAddressOrUsername);

        builder
            .HasIndex(x => new
            {
                x.UserId,
                x.IsActiveSession,
                x.Status
            });
    }
}