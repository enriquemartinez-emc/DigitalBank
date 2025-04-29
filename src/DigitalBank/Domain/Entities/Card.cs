namespace DigitalBank.Domain.Entities;

public class Card
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AccountId { get; private set; }
    public string CardNumber { get; private set; }
    public string ExpiryDate { get; private set; }
    public string CVV { get; private set; }
    public Account Account { get; private set; } = null!;

    public Card(Guid accountId, string cardNumber, string expiryDate, string cvv)
    {
        AccountId = accountId;
        CardNumber = cardNumber;
        ExpiryDate = expiryDate;
        CVV = cvv;
    }
}
