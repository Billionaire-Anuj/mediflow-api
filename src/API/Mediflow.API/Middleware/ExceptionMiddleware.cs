using Newtonsoft.Json;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.Response;
using System.ComponentModel.DataAnnotations;

namespace Mediflow.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var response = exception switch
        {
            BadRequestException badRequestException => new ResponseDto<object>((int)HttpStatusCode.BadRequest, badRequestException.Message, null),
            CustomException customException => new ResponseDto<object>((int)HttpStatusCode.InternalServerError, customException.Message, null),
            NotFoundException notFoundException => new ResponseDto<object>((int)HttpStatusCode.NotFound, notFoundException.Message, null),
            PartialException partialException => new ResponseDto<object>((int)HttpStatusCode.Conflict, partialException.Message, null),
            UnauthorizedException unauthorizedException => new ResponseDto<object>(unauthorizedException.Status, unauthorizedException.Message, null),
            ForbiddenException forbiddenException => new ResponseDto<object>(forbiddenException.Status, forbiddenException.Message, null),
            ValidationException validationException => new ResponseDto<object>((int)HttpStatusCode.BadRequest, validationException.Message, null),
            _ => new ResponseDto<object>((int)HttpStatusCode.InternalServerError, exception.Message, null)
        };

        var statusCode = (HttpStatusCode)response.StatusCode;
        
        httpContext.Response.StatusCode = (int)statusCode;

        var logMessage = JsonConvert.SerializeObject(exception);

        logger.LogError(logMessage);

        await httpContext.Response.WriteAsJsonAsync(response);
    }
}