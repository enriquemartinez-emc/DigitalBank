using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;

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
            return Result.Failure<Account>(Errors.Account.InvalidAccountNumber);

        if (initialBalance < 0)
            return Result.Failure<Account>(Errors.Account.InvalidInitialBalance);

        var account = new Account
        {
            AccountNumber = accountNumber,
            CustomerId = customerId,
            Balance = initialBalance
        };

        return Result.Success(account);
    }

    public Result UpdateBalance(decimal amount, TransactionType transactionType)
    {
        if (amount <= 0)
            return Result.Failure(Errors.Account.InvalidAmount);

        return transactionType switch
        {
            TransactionType.Deposit => ApplyDeposit(amount),
            TransactionType.Withdrawal => ApplyWithdrawal(amount),
            _ => Result.Failure(Errors.Account.InvalidTransactionType)
        };
    }

    private Result ApplyDeposit(decimal amount)
    {
        Balance += amount;
        return Result.Success();
    }

    private Result ApplyWithdrawal(decimal amount)
    {
        if (Balance < amount)
            return Result.Failure(Errors.Account.InsuficientFunds);

        Balance -= amount;
        return Result.Success();
    }
}
