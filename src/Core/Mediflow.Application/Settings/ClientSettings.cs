using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Settings;

public class ClientSettings : IValidatableObject
{
    public string BaseUrl { get; set; } = string.Empty;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(BaseUrl))
        {
            yield return new ValidationResult(
                $"{nameof(ClientSettings)} is not configured.",
                [nameof(BaseUrl)]);
        }
    }
}