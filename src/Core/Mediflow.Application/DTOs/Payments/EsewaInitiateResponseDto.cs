namespace Mediflow.Application.DTOs.Payments;

public class EsewaInitiateResponseDto
{
    public string PaymentUrl { get; set; } = string.Empty;

    public string TransactionUuid { get; set; } = string.Empty;

    public Dictionary<string, string> Payload { get; set; } = new();
}
