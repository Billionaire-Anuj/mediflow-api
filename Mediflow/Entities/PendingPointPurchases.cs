using System.ComponentModel.DataAnnotations;
using Mediflow.Entities.Enums;

namespace mediflow.Entities;

public sealed class PendingPointPurchases
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int Points { get; set; }

    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "NPR";

    public PaymentProvider Provider { get; set; }

    [MaxLength(120)]
    public string ProviderReference { get; set; } = "";

    public PurchaseStatus Status { get; set; } = PurchaseStatus.Pending;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}