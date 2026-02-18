using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mediflow.API.Middleware;

public class RequestNormalizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType?.Contains("application/json") == true)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body);

            var bodyText = await reader.ReadToEndAsync();

            context.Request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(bodyText))
            {
                var json = JsonNode.Parse(bodyText);

                NormalizeJson(json);

                var normalized = json!.ToJsonString(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                var bytes = System.Text.Encoding.UTF8.GetBytes(normalized);

                context.Request.Body = new MemoryStream(bytes);
            }
        }

        await next(context);
    }

    private static void NormalizeJson(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject @object:
            {
                foreach (var property in @object.ToList())
                {
                    if (property.Value is JsonValue jsonValue && jsonValue.TryGetValue(out string? value))
                    {
                        var trimmed = value.Trim();

                        @object[property.Key] = string.IsNullOrEmpty(trimmed) ? null : trimmed;
                    }
                    else
                    {
                        NormalizeJson(property.Value);
                    }
                }

                break;
            }
            case JsonArray array:
            {
                foreach (var jsonNode in array)
                {
                    NormalizeJson(jsonNode);
                }

                break;
            }
        }
    }
}