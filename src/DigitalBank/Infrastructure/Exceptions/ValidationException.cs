using DigitalBank.Infrastructure.Validation;

namespace DigitalBank.Infrastructure.Exceptions;

public class ValidationException : Exception
{
    public ValidationProblemDetails ProblemDetails { get; }

    public ValidationException(ValidationProblemDetails problemDetails)
        : base(problemDetails.Title)
    {
        ProblemDetails = problemDetails;
    }
}