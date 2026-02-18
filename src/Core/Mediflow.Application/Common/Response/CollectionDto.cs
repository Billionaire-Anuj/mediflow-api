namespace Mediflow.Application.Common.Response;

public class CollectionDto<T>(int statusCode, string message, List<T> items, int count, int pageNumber, int pageSize) where T : class
{
    public int StatusCode { get; private set; } = statusCode;

    public string Message { get; private set; } = message;

    public int CurrentPage { get; private set; } = pageNumber;

    public int TotalPages { get; private set; } = count != 0 ? (int)Math.Ceiling(count / (double)pageSize) : 0;

    public int PageSize { get; private set; } = pageSize;

    public int TotalCount { get; private set; } = count;

    public int DisplayCount { get; private set; } = items.Count;

    public IEnumerable<T> Result { get; private set; } = items;
}