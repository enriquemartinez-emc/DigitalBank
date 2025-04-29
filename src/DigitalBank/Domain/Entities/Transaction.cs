namespace DigitalBank.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Account Account { get; private set; } = null!;
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer,
    Payment
}
