using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class ResourceConfigurations : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder
            .Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired(false);
        
        builder
            .Property(x => x.Description)
            .HasMaxLength(150)
            .IsRequired(false);
        
        builder
            .HasMany(x => x.Permissions)
            .WithOne(t => t.Resource)
            .HasForeignKey(t => t.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}