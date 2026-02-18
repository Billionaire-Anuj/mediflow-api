using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace Mediflow.Application.Common.Configuration;

public class SlugifyParameterConfiguration : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value is null) return null;
        var s = value.ToString()!;
        if (s.Length == 0) return s;

        s = Regex.Replace(s, "([A-Z]+)([A-Z][a-z])", "$1/$2");
        s = Regex.Replace(s, "([a-z0-9])([A-Z])", "$1/$2");
        s = Regex.Replace(s, "([A-Z])([0-9])", "$1/$2");
        s = Regex.Replace(s, "([0-9])([A-Z])", "$1/$2");
        s = s.Replace('_', '/');

        return s.ToLowerInvariant();
    }
}