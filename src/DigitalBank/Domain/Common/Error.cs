using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Domain.Common;

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

public static class Errors
{
    public static class Account
    {
        public static Error InvalidAccountNumber() => new("Account.InvalidNumber", "Account number cannot be empty");
        public static Error InvalidInitialBalance() => new("Account.InvalidBalance", "Initial balance cannot be negative");
        public static Error InvalidAmount() => new("Account.InvalidAmount", "Amount must be positive");
        public static Error NotFound() => new("Account.NotFound", "Account not found");
    }
}
