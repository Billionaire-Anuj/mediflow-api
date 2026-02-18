using System.ComponentModel.DataAnnotations;

namespace Mediflow.Application.Common.Attributes;

public class PasswordExaminationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password)
        {
            return new ValidationResult("A valid alphanumeric password is required.");
        }

        if (password.Length < 8)
        {
            return new ValidationResult("Password must be at least 8 characters long.");
        }

        if (!RegexValidator.NumericExaminationRegex().IsMatch(password))
        {
            return new ValidationResult("Password must contain at least one digit (0-9).");
        }

        if (!RegexValidator.UpperCaseCharacterExaminationRegex().IsMatch(password))
        {
            return new ValidationResult("Password must contain at least one uppercase letter (A-Z).");
        }

        if (!RegexValidator.LowerCaseCharacterExaminationRegex().IsMatch(password))
        {
            return new ValidationResult("Password must contain at least one lowercase letter (a-z).");
        }

        if (!RegexValidator.SpecialCharacterExaminationRegex().IsMatch(password))
        {
            return new ValidationResult("Password must contain at least one special character (e.g., @, #, $, etc.).");
        }

        return ValidationResult.Success;
    }
}