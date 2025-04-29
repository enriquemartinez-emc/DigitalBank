namespace DigitalBank.Domain.Entities;

public class Transfer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Account FromAccount { get; set; } = null!;
    public Account ToAccount { get; set; } = null!;
}
