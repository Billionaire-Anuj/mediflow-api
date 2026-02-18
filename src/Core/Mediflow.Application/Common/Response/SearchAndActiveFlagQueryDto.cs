using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Common.Response;

public class SearchAndActiveFlagQueryDto : IValidatableObject
{
    public string? GlobalSearch { get; set; }

    public bool[]? IsActive { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }
}