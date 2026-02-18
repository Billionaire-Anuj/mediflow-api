using Mediflow.Domain.Entities;
using Newtonsoft.Json;
using Mediflow.Domain.Common.Property;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class UserConfigurationConfigurations : IEntityTypeConfiguration<UserConfiguration>
{
    public void Configure(EntityTypeBuilder<UserConfiguration> builder)
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