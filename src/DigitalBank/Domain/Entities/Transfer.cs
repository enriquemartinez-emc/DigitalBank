namespace DigitalBank.Domain.Entities;

public class Transfer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid FromAccountId { get; private set; }
    public Guid ToAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Account FromAccount { get; private set; } = null!;
    public Account ToAccount { get; private set; } = null!;
}
