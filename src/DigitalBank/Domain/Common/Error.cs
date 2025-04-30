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
    public static class Customer
    {
        public static Error InvalidFirstName => new("Customer.InvalidFirstName", "First name must be 1-50 characters and contain only letters, spaces, or hyphens.");
        public static Error InvalidLastName => new("Customer.InvalidLastName", "Last name must be 1-50 characters and contain only letters, spaces, or hyphens.");
        public static Error InvalidEmail => new("Customer.InvalidEmail", "Email must be a valid email address.");
        public static Error DuplicateEmail => new("Customer.DuplicateEmail", "Email is already in use.");
        public static Error NotFound => new("Customer.NotFound", "Customer not found.");
    }

    public static class Card
    {
        public static Error InvalidAccount => new("Card.InvalidAccount", "Account ID is invalid.");
        public static Error TooManyCards => new("Card.TooManyCards", "Account cannot have more than 3 cards.");
        public static Error InvalidCardHolderName => new("Card.InvalidCardHolderName", "Card holder name must match customer's full name.");
        public static Error NotFound => new("Card.NotFound", "Card not found.");
    }

    public static class Account
    {
        public static Error InvalidAccountNumber => new("Account.InvalidNumber", "Account number cannot be empty");
        public static Error InvalidInitialBalance => new("Account.InvalidBalance", "Initial balance cannot be negative");
        public static Error InvalidAmount => new("Account.InvalidAmount", "Amount must be positive");
        public static Error NotFound => new("Account.NotFound", "Account not found");
    }
}
