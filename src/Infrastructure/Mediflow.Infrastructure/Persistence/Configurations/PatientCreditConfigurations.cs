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
            .HasOne(x => x.CreatedUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.LastModifiedUser)
            .WithMany()
            .HasForeignKey(x => x.LastModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.DeletedUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => x.PatientId)
            .IsUnique();
    }
}
