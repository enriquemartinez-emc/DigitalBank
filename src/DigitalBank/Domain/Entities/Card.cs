namespace DigitalBank.Domain.Entities;

public class Card
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AccountId { get; private set; }
    public required string CardNumber { get; set; }
    public required string ExpiryDate { get; set; }
    public required string CVV { get; set; }
    public Account Account { get; private set; } = null!;
}
