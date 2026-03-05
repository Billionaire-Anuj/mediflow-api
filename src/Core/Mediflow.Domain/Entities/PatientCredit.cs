using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class PatientCredit(Guid patientId, decimal creditPoints, string paymentIndex) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Patient))]
    public Guid PatientId { get; private set; } = patientId;

    public decimal CreditPoints { get; private set; } = creditPoints;

    public string PaymentIndex { get; private set; } = paymentIndex;

    public static PatientCredit Default => new(Guid.Empty, 0m, string.Empty);

    public virtual User? Patient { get; set; }

    public void AddCredits(decimal credits, string paymentIndex)
    {
        if (credits <= 0)
        {
            return;
        }

        CreditPoints += credits;
        PaymentIndex = paymentIndex;
    }
}
