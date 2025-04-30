using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Domain.Common.Errors;

public record Error(string Code, string Message)
{
    public ProblemDetails ToProblemDetails(int statusCode) => new()
    {
        Status = statusCode,
        Type = Code,
        Title = Message,
        Detail = Message
    };
}
