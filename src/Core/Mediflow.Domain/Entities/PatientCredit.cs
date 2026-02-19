using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class PatientCredit(Guid patientId, decimal creditPoints, string paymentIndex) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Patient))]
    public Guid PatientId { get; private set; } = patientId;

    public decimal CreditPoints { get; private set; } = creditPoints;

    public string PaymentIndex { get; private set; } = paymentIndex;

    public virtual User? Patient { get; set; }
}