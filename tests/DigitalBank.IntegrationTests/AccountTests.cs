using DigitalBank.Application.Features.Accounts;
using DigitalBank.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalBank.IntegrationTests;

public class AccountIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateAccount_ValidInput_CreatesAccountAndReturnsId()
    {
        // Arrange
        var customer = Customer.Create("Jane", "Doe", "jane.doe@example.com").Value!;

        using (var dbContext = await CreateDbContextAsync())
        {
            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();
        }

        var accountData = new AccountData(
            AccountNumber: "12345678901234",
            InitialBalance: 1000m);

        var command = new CreateAccountCommand(
            AccountData: accountData,
            CustomerId: customer.Id);

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        using var assertDbContext = await CreateDbContextAsync();
        var account = await assertDbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == result.Value);

        Assert.NotNull(account);
        Assert.Equal(command.AccountData.AccountNumber, account.AccountNumber);
        Assert.Equal(command.CustomerId, account.CustomerId);
        Assert.Equal(command.AccountData.InitialBalance, account.Balance);
    }

    [Fact]
    public async Task CreateAccount_InvalidCustomerId_ReturnsFailure()
    {
        // Arrange
        var accountData = new AccountData(
            AccountNumber: "12345678901234",
            InitialBalance: 1000m);

        var command = new CreateAccountCommand(
            AccountData: accountData,
            CustomerId: Guid.NewGuid()); // non-existent customer ID

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Account.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetAccountBalance_ValidAccountId_ReturnsBalance()
    {
        var customer = Customer.Create("John", "Doe", "john.doe@example.com").Value!;
        var account = Account.Create("12345678901234", customer.Id, 500m).Value!;

        using (var dbContext = await CreateDbContextAsync())
        {
            dbContext.Customers.Add(customer);
            dbContext.Accounts.Add(account);
            await dbContext.SaveChangesAsync();
        }

        var query = new GetAccountBalanceQuery(account.Id, customer.Id);

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(500m, result.Value);
    }

    [Fact]
    public async Task GetAccountBalance_InvalidAccountId_ReturnsFailure()
    {
        // Arrange
        var query = new GetAccountBalanceQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(query);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Account.NotFound", result.Error.Code);
    }
}