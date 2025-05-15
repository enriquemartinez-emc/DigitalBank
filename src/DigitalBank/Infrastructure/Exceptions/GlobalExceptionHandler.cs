using DigitalBank.Domain.Common.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Infrastructure.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        (ProblemDetails, int) result = exception switch
        {
            ValidationException validationException => (MapValidationProblemDetails(validationException.ProblemDetails), StatusCodes.Status400BadRequest),
            InvalidOperationException => (new Error("Operation.Invalid", exception.Message).ToProblemDetails(StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest),
            _ => (new Error("Server.Error", "An unexpected error occurred").ToProblemDetails(StatusCodes.Status500InternalServerError), StatusCodes.Status500InternalServerError)
        };

        (ProblemDetails problemDetails, int statusCode) = result;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static ProblemDetails MapValidationProblemDetails(Validation.ValidationProblemDetails validationProblemDetails)
    {
        return new ProblemDetails
        {
            Title = validationProblemDetails.Title,
            Status = validationProblemDetails.Status,
            Detail = validationProblemDetails.Detail,
            Extensions = { ["errors"] = validationProblemDetails.Errors }
        };
    }
}