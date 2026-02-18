namespace Mediflow.Application.Common.Response;

public class ResponseDto<T>(int statusCode, string message, T? result)
{
    public int StatusCode { get; private set; } = statusCode;

    public string Message { get; private set; } = message;

    public T? Result { get; private set; } = result;
}
