using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Common.Response;

public class OrderQueryDto : IValidatableObject
{
    public string[]? OrderBys { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }
}