using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Common.Response;

public class PaginationQueryDto : IValidatableObject
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 25;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PageNumber < 1 || PageSize < 1)
        {
            yield return new ValidationResult(
                "Pagination is not configured.",
                [
                    nameof(PageNumber),
                    nameof(PageSize)
                ]);
        }
    }
}