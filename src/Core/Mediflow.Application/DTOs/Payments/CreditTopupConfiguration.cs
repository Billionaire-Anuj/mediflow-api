namespace Mediflow.Application.DTOs.Payments;

public class CreditTopupConfiguration
{
    public decimal Amount { get; set; }

    public string Provider { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
