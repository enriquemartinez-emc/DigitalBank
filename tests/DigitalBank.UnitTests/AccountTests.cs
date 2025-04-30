using DigitalBank.Domain.Entities;

namespace DigitalBank.UnitTests;

public class AccountTests
{
    [Fact]
    public void CreateAccount_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var accountNumber = "1234567890";
        var customerId = Guid.NewGuid();

        // Act
        var result = Account.Create(accountNumber, customerId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(accountNumber, result.Value.AccountNumber);
        Assert.Equal(customerId, result.Value.CustomerId);
    }

    [Fact]
    public void CreateAccount_InvalidAccountNumber_ReturnsFailure()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var result = Account.Create("", customerId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Account.InvalidNumber", result.Error.Code);
    }
}