using System.Reflection;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Mediflow.Domain.Common.Enum;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Mediflow.Helper;

public static class GenericExtensionMethods
{
    private const string Prefix = "GVAV";
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string SetUniqueFileName(this string fileExtension)
    {
        return $"{DateTime.Now:ddMMyyyyHHmmssfff}" + fileExtension;
    }

    public static FileType? GetFileType(this IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        foreach (FileType fileType in Enum.GetValues(typeof(FileType)))
        {
            var description = GetEnumDescription(fileType);
            var allowedExtensions = description.Split(',');

            if (allowedExtensions.Contains(fileExtension))
            {
                return fileType;
            }
        }

        return null;
    }

    public static long ToUnixTimeMilliSeconds(this DateTime dateTime)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime.ToUniversalTime());
        
        return dateTimeOffset.ToUnixTimeMilliseconds();
    }

    public static bool Matches(this string? source, string? term) =>
        !string.IsNullOrEmpty(term) && source?.ToLowerInvariant().Contains(term.ToLowerInvariant()) == true;

    public static string ToDisplayName(this string name, Type? type = null)
    {
        var fieldName = name;

        if (fieldName != "Id" && fieldName.EndsWith("Id", StringComparison.Ordinal) && type == typeof(Guid))
            fieldName = fieldName[..^2];

        fieldName = fieldName.Replace('_', ' ').Replace('.', ' ');

        fieldName = Regex.Replace(fieldName, "([a-z0-9])([A-Z])", "$1 $2");

        fieldName = Regex.Replace(fieldName, "([A-Z]+)([A-Z][a-z])", "$1 $2");

        fieldName = Regex.Replace(fieldName, @"\s+", " ").Trim();

        return fieldName;
    }

    public static string ToDisplayNameField(this string title)
    {
        return Regex.Replace(title, "(?<!^)([A-Z])", " $1");
    }

    private static string GetEnumDescription(FileType value)
    {
        var field = value.GetType().GetField(value.ToString());

        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? string.Empty;
    }

    public static async Task<string> ComputeSha256Async(this Stream stream, CancellationToken cancellationToken)
    {
        using var sha = SHA256.Create();

        var hashBytes = await sha.ComputeHashAsync(stream, cancellationToken);

        stream.Position = 0;

        return Convert.ToHexString(hashBytes);
    }

    public static string ToFileSize(this long bytes)
    {
        string[] sizes = { "Bytes", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    public static string ToContentType(this string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    public static string ToAge(this DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (today < dateOfBirth)
            throw new ArgumentOutOfRangeException(nameof(dateOfBirth), "Date of birth cannot be in the future.");

        var years = today.Year - dateOfBirth.Year;
        var yearAnniversary = dateOfBirth.AddYears(years);

        if (yearAnniversary > today)
        {
            years--;
            yearAnniversary = dateOfBirth.AddYears(years);
        }

        var months = (today.Year - yearAnniversary.Year) * 12 + (today.Month - yearAnniversary.Month);
        var monthAnniversary = yearAnniversary.AddMonths(months);

        if (monthAnniversary > today)
        {
            months--;
            monthAnniversary = yearAnniversary.AddMonths(months);
        }

        var days = today.DayNumber - monthAnniversary.DayNumber;

        return $"{years} years {months} months {days} day(s)";
    }

    public static string GenerateIdentifier() => $"{Prefix}-{Segment(4)}-{Segment(4)}";

    private static string Segment(int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];

        return new string(chars);
    }
}