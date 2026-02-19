using Newtonsoft.Json;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mediflow.Domain.Common.Property;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class UserPropertyConfigurations : IEntityTypeConfiguration<UserProperty>
{
    public void Configure(EntityTypeBuilder<UserProperty> builder)
    {
        builder
            .Property(x => x.UserId)
            .IsRequired();

        builder
            .Property(x => x.Configurations)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v) ?? new KeyValueProperty())
            .HasColumnType("jsonb")
            .IsRequired();
    }
}