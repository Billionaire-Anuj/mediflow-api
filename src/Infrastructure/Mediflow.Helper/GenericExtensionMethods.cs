using System.Text;
using System.Text.RegularExpressions;

namespace Mediflow.Helper;

public static class GenericExtensionMethods
{
    public static string SetUniqueFileName(this string fileExtension)
    {
        return $"{DateTime.Now:ddMMyyyyHHmmssfff}" + fileExtension;
    }

    public static long ToUnixTimeMilliSeconds(this DateTime dateTime)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime.ToUniversalTime());
        
        return dateTimeOffset.ToUnixTimeMilliseconds();
    }

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

    public static string[] ParseCsvLine(this string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return Array.Empty<string>();

        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                fields.Add(current.ToString().Trim());
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        fields.Add(current.ToString().Trim());

        return fields.ToArray();
    }
}