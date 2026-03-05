using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Settings;

public class KhaltiSettings : IValidatableObject
{
    public string PublicKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string InitiateUrl { get; set; } = string.Empty;

    public string LookupUrl { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;

    public string WebsiteUrl { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(SecretKey))
        {
            yield return new ValidationResult(
                $"{nameof(KhaltiSettings)}.{nameof(SecretKey)} is not configured.",
                [nameof(SecretKey)]);
        }

        if (string.IsNullOrWhiteSpace(InitiateUrl))
        {
            yield return new ValidationResult(
                $"{nameof(KhaltiSettings)}.{nameof(InitiateUrl)} is not configured.",
                [nameof(InitiateUrl)]);
        }

        if (string.IsNullOrWhiteSpace(LookupUrl))
        {
            yield return new ValidationResult(
                $"{nameof(KhaltiSettings)}.{nameof(LookupUrl)} is not configured.",
                [nameof(LookupUrl)]);
        }

        if (string.IsNullOrWhiteSpace(ReturnUrl))
        {
            yield return new ValidationResult(
                $"{nameof(KhaltiSettings)}.{nameof(ReturnUrl)} is not configured.",
                [nameof(ReturnUrl)]);
        }

        if (string.IsNullOrWhiteSpace(WebsiteUrl))
        {
            yield return new ValidationResult(
                $"{nameof(KhaltiSettings)}.{nameof(WebsiteUrl)} is not configured.",
                [nameof(WebsiteUrl)]);
        }
    }
}
