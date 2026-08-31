using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using N1che.Domain.Exceptions;

namespace N1che.Api.ExceptionHandling;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            DuplicateRequestException => (StatusCodes.Status409Conflict, exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        var isUnexpected = statusCode == StatusCodes.Status500InternalServerError;

        if (isUnexpected)
        {
            logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            logger.LogWarning(exception, "{Title}", title);
        }

        httpContext.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = isUnexpected ? "An unexpected error occurred. Please try again later." : exception.Message,
            Instance = httpContext.Request.Path
        };
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
