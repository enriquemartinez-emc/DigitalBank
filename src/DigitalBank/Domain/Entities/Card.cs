using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;

namespace DigitalBank.Domain.Entities;

public enum CardType
{
    Debit,
    Credit
}

public class Card
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public CardType CardType { get; private set; }
    public string CardHolderName { get; private set; } = string.Empty;
    public string CardNumber { get; private set; } = string.Empty;
    public DateTime ExpiryDate { get; private set; }
    public bool IsActive { get; private set; }

    private Card() { } // For EF Core

    public static Result<Card> Create(
        Guid accountId,
        CardType cardType,
        string cardHolderName,
        string customerFullName,
        int accountCardCount)
    {
        if (accountId == Guid.Empty)
            return Result.Failure<Card>(Errors.Card.InvalidAccount);

        if (accountCardCount >= 3)
            return Result.Failure<Card>(Errors.Card.TooManyCards);

        if (string.IsNullOrWhiteSpace(cardHolderName) || cardHolderName.Trim() != customerFullName.Trim())
            return Result.Failure<Card>(Errors.Card.InvalidCardHolderName);

        var cardNumber = GenerateCardNumber();
        var expiryDate = DateTime.UtcNow.AddYears(3);

        return Result.Success(new Card
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            CardType = cardType,
            CardHolderName = cardHolderName.Trim(),
            CardNumber = cardNumber,
            ExpiryDate = expiryDate,
            IsActive = true
        });
    }

    private static string GenerateCardNumber()
    {
        var random = new Random();
        var digits = new char[16];
        for (int i = 0; i < 16; i++)
        {
            digits[i] = (char)('0' + random.Next(0, 10));
        }

        return new string(digits);
    }
}
