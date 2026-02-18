using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class RoleConfigurations : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();
        
        builder
            .Property(x => x.Description)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(x => x.IsDisplayed)
            .HasDefaultValue(true)
            .IsRequired();

        builder
            .Property(x => x.IsRegisterable)
            .HasDefaultValue(true)
            .IsRequired();

        builder
            .HasMany(x => x.Users)
            .WithOne(t => t.Role)
            .HasForeignKey(t => t.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasMany(x => x.Permissions)
            .WithOne(t => t.Role)
            .HasForeignKey(t => t.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(r => r.Name)
            .IsUnique();
    }
}