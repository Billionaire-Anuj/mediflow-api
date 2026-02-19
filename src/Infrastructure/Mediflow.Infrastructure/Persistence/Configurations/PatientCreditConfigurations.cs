using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediflow.Infrastructure.Persistence.Configurations;

public sealed class PatientCreditConfigurations : IEntityTypeConfiguration<PatientCredit>
{
    public void Configure(EntityTypeBuilder<PatientCredit> builder)
    {
        builder
            .Property(x => x.PatientId)
            .IsRequired();

        builder
            .Property(x => x.CreditPoints)
            .HasPrecision(18, 2)
            .IsRequired();

        builder
            .Property(x => x.PaymentIndex)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .HasOne(x => x.Patient)
            .WithOne(x => x.Credit)
            .HasForeignKey<PatientCredit>(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.PatientId)
            .IsUnique();
    }
}
