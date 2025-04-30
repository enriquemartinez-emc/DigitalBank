namespace DigitalBank.Domain.Common.Errors;

public static partial class Errors
{
    public static class Account
    {
        public static Error InvalidAccountNumber => new("Account.InvalidNumber", "Account number cannot be empty");
        public static Error InvalidInitialBalance => new("Account.InvalidBalance", "Initial balance cannot be negative");
        public static Error InvalidAmount => new("Account.InvalidAmount", "Amount must be positive");
        public static Error NotFound => new("Account.NotFound", "Account not found");
        public static Error InsuficientFunds => new("Account.InsufficientFunds", "Insufficient funds for this transaction");
        public static Error InvalidTransactionType => new("Account.InvalidTransactionType", "Invalid transaction type");
    }
}
