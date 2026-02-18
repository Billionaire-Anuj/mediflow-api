namespace Mediflow.Application.Common.Filter;

public class Search
{
    public string? Keyword { get; set; }
    
    public List<string> Fields { get; set; } = new();
}