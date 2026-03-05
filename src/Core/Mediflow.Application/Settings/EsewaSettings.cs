using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Settings;

public class EsewaSettings : IValidatableObject
{
    public string MerchantCode { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string InitiateUrl { get; set; } = string.Empty;

    public string SuccessUrl { get; set; } = string.Empty;

    public string FailureUrl { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(MerchantCode))
        {
            yield return new ValidationResult(
                $"{nameof(EsewaSettings)}.{nameof(MerchantCode)} is not configured.",
                [nameof(MerchantCode)]);
        }

        if (string.IsNullOrWhiteSpace(SecretKey))
        {
            yield return new ValidationResult(
                $"{nameof(EsewaSettings)}.{nameof(SecretKey)} is not configured.",
                [nameof(SecretKey)]);
        }

        if (string.IsNullOrWhiteSpace(InitiateUrl))
        {
            yield return new ValidationResult(
                $"{nameof(EsewaSettings)}.{nameof(InitiateUrl)} is not configured.",
                [nameof(InitiateUrl)]);
        }

        if (string.IsNullOrWhiteSpace(SuccessUrl))
        {
            yield return new ValidationResult(
                $"{nameof(EsewaSettings)}.{nameof(SuccessUrl)} is not configured.",
                [nameof(SuccessUrl)]);
        }

        if (string.IsNullOrWhiteSpace(FailureUrl))
        {
            yield return new ValidationResult(
                $"{nameof(EsewaSettings)}.{nameof(FailureUrl)} is not configured.",
                [nameof(FailureUrl)]);
        }
    }
}
