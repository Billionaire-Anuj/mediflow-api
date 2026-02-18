using System.Text.RegularExpressions;

namespace Mediflow.Application.Common.Attributes;

public static partial class RegexValidator
{
    [GeneratedRegex(@"\d")]
    public static partial Regex NumericExaminationRegex();
    
    [GeneratedRegex("[A-Z]")]
    public static partial Regex UpperCaseCharacterExaminationRegex();
    
    [GeneratedRegex("[a-z]")]
    public static partial Regex LowerCaseCharacterExaminationRegex();
    
    [GeneratedRegex(@"[^a-zA-Z\d]")]
    public static partial Regex SpecialCharacterExaminationRegex();
}