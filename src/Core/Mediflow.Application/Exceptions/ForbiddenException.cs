using System.Net;

namespace Mediflow.Application.Exceptions;

public class ForbiddenException(string message, object? result = null) : Exception(message)
{
    public int Status { get; private set; } = (int)HttpStatusCode.Forbidden;
    
    public object? Result { get; private set; } = result;
}