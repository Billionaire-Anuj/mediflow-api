using Newtonsoft.Json;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mediflow.Domain.Common.Property;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class PropertyConfigurations : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder
            .Property(x => x.Setting)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v) ?? new KeyValueProperty())
            .HasColumnType("jsonb")
            .IsRequired();
    }
}