using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfigurations : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder
            .Property(x => x.RoleId)
            .IsRequired();

        builder
            .Property(x => x.ResourceId)
            .IsRequired();
        
        builder
            .HasOne(x => x.Resource)
            .WithMany(x => x.Permissions) 
            .HasForeignKey(x => x.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Role)
            .WithMany(x => x.Permissions) 
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Property(x => x.Action)
            .HasMaxLength(150)
            .IsRequired(false);
        
        builder
            .HasIndex(x => new
            {
                x.Action,
                x.ResourceId,
                x.RoleId
            })
            .IsUnique();
    }
}