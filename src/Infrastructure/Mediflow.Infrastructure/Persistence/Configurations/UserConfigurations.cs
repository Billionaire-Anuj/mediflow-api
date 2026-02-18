using System.Text.Json;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(x => x.RoleId)
            .IsRequired();

        builder
            .Property(x => x.Gender)
            .HasConversion<string>()
            .IsRequired();

        builder
            .Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(x => x.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(x => x.EmailAddress)
            .HasMaxLength(150)
            .IsRequired();

        builder
            .Property(x => x.Address)
            .HasMaxLength(250)
            .IsRequired(false);

        builder
            .Property(x => x.ProfileImage)
            .HasConversion(
                v => JsonSerializer.Serialize(v, AssetConfigurations.JsonOptions),
                v => JsonSerializer.Deserialize<Asset>(v, AssetConfigurations.JsonOptions)!)
            .HasColumnType("jsonb")
            .IsRequired(false);
        
        builder
            .Property(x => x.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder
            .HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.AuditLogs)
            .WithOne(r => r.Auditor)
            .HasForeignKey(x => x.AuditorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.UserLoginLogs)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.Username)
            .IsUnique();

        builder
            .HasIndex(x => x.EmailAddress)
            .IsUnique();

        builder
            .HasIndex(x => x.PhoneNumber)
            .IsUnique();

        builder
            .HasIndex(x => x.RoleId);
    }
}