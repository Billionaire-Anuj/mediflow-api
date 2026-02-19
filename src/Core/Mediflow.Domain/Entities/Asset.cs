using Mediflow.Domain.Common.Enum;
using System.Text.Json.Serialization;

namespace Mediflow.Domain.Entities;

public class Asset(string fileUrl, string originalFileName, double? aspectRatio = null, Orientation? orientation = null)
{
    [JsonPropertyName("FileUrl")]
    public string FileUrl { get; private set; } = fileUrl;

    [JsonPropertyName("OriginalFileName")]
    public string OriginalFileName { get; private set; } = originalFileName;

    [JsonPropertyName("AspectRatio")]
    public double? AspectRatio { get; private set; } = aspectRatio;

    [JsonPropertyName("Orientation")]
    public Orientation? Orientation { get; private set; } = orientation;

    public static Asset Default => new(string.Empty, string.Empty);
}