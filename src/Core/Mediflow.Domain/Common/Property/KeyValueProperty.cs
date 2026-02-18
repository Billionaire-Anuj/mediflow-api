namespace Mediflow.Domain.Common.Property;

public class KeyValueProperty
{
    public string Key { get; set; } = string.Empty;

    public object Value { get; set; } = new();
}