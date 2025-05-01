using DigitalBank.Domain.Common.Errors;
using Microsoft.AspNetCore.Diagnostics;

namespace DigitalBank.Infrastructure.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var error = exception switch
        {
            InvalidOperationException => new Error("Operation.Invalid", exception.Message),
            _ => new Error("Server.Error", "An unexpected error occurred")
        };

        var problemDetails = error.ToProblemDetails(StatusCodes.Status500InternalServerError);
        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
