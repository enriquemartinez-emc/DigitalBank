using DigitalBank.Domain.Entities;

namespace DigitalBank.UnitTests;

public class CustomerTests
{
    [Fact]
    public void CreateCustomer_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var firstName = "Jane";
        var lastName = "Doe";
        var email = "jane.doe@example.com";

        // Act
        var result = Customer.Create(firstName, lastName, email);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(firstName, result.Value!.FirstName); // Added null-forgiving operator
        Assert.Equal(lastName, result.Value.LastName);
        Assert.Equal(email.ToLower(), result.Value.Email);
    }

    [Fact]
    public void CreateCustomer_InvalidEmail_ReturnsFailure()
    {
        // Arrange
        var firstName = "Jane";
        var lastName = "Doe";
        var email = "invalid-email";

        // Act
        var result = Customer.Create(firstName, lastName, email);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error); // Added null check
        Assert.Equal("Customer.InvalidEmail", result.Error!.Code); // Added null-forgiving operator
    }

    [Fact]
    public void CreateCustomer_InvalidFirstName_ReturnsFailure()
    {
        // Arrange
        var firstName = "Jane123";
        var lastName = "Doe";
        var email = "jane.doe@example.com";

        // Act
        var result = Customer.Create(firstName, lastName, email);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error); // Added null check
        Assert.Equal("Customer.InvalidFirstName", result.Error!.Code); // Added null-forgiving operator
    }
}
