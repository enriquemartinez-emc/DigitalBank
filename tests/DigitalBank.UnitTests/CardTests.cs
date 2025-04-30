using DigitalBank.Domain.Entities;

namespace DigitalBank.UnitTests;

public class CardTests
{
    [Fact]
    public void CreateCard_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var cardType = CardType.Debit;
        var cardHolderName = "John Doe";
        var customerFullName = "John Doe";
        var accountCardCount = 0;

        // Act
        var result = Card.Create(
            accountId,
            cardType,
            cardHolderName,
            customerFullName,
            accountCardCount);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(accountId, result.Value.AccountId);
        Assert.Equal(cardType, result.Value.CardType);
        Assert.Equal(cardHolderName, result.Value.CardHolderName);
        Assert.Equal(16, result.Value.CardNumber.Length);
        Assert.True(result.Value.IsActive);
    }

    [Fact]
    public void CreateCard_TooManyCards_ReturnsFailure()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var cardType = CardType.Debit;
        var cardHolderName = "John Doe";
        var customerFullName = "John Doe";
        var accountCardCount = 3;

        // Act
        var result = Card.Create(accountId, cardType, cardHolderName, customerFullName, accountCardCount);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Card.TooManyCards", result.Error!.Code);
    }

    [Fact]
    public void CreateCard_InvalidCardHolderName_ReturnsFailure()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var cardType = CardType.Debit;
        var cardHolderName = "Jane Doe";
        var customerFullName = "John Doe";
        var accountCardCount = 0;

        // Act
        var result = Card.Create(
            accountId,
            cardType,
            cardHolderName,
            customerFullName,
            accountCardCount);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Card.InvalidCardHolderName", result.Error!.Code);
    }
}
