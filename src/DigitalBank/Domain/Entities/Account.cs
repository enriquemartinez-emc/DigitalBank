using DigitalBank.Domain.Common;

namespace DigitalBank.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string AccountNumber { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public List<Transaction> Transactions { get; private set; } = [];
    public List<Card> Cards { get; private set; } = [];

    public static Result<Account> Create(string accountNumber, Guid customerId, decimal initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return Result<Account>.Failure<Account>(Errors.Account.InvalidAccountNumber);

        if (initialBalance < 0)
            return Result<Account>.Failure<Account>(Errors.Account.InvalidInitialBalance);

        var account = new Account
        {
            AccountNumber = accountNumber,
            CustomerId = customerId,
            Balance = initialBalance
        };

        return Result<Account>.Success(account);
    }

    public Result UpdateBalance(decimal amount, TransactionType transactionType)
    {
        if (amount <= 0)
            return Result.Failure(Errors.Account.InvalidAmount);

        Balance = transactionType switch
        {
            TransactionType.Deposit => Balance + amount,
            TransactionType.Withdrawal => Balance >= amount ? Balance - amount : throw new InvalidOperationException("Insufficient funds"),
            _ => throw new ArgumentException("Invalid transaction type")
        };

        return Result.Success();
    }
}
