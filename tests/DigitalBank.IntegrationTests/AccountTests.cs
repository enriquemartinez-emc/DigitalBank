using DigitalBank.Application.Features.Accounts;
using DigitalBank.Domain.Common;
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
        var result = await SendRequestAsync<CreateAccountCommand, Guid>(command);

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
        var result = await SendRequestAsync<CreateAccountCommand, Guid>(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Account.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetAccount_ValidAccountId_ReturnsAccount()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "john.doe@example.com").Value!;
        var account = Account.Create("12345678901234", customer.Id, 500m).Value!;

        using (var dbContext = await CreateDbContextAsync())
        {
            dbContext.Customers.Add(customer);
            dbContext.Accounts.Add(account);
            await dbContext.SaveChangesAsync();
        }

        var query = new GetAccountQuery(account.Id, customer.Id);

        // Act
        var result = await SendRequestAsync<GetAccountQuery, Account>(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result);
        Assert.Equal(account.Id, result.Value!.Id);
        Assert.Equal(account.AccountNumber, result.Value.AccountNumber);
        Assert.Equal(account.CustomerId, result.Value.CustomerId);
        Assert.Equal(account.Balance, result.Value.Balance);
    }

    [Fact]
    public async Task GetAccount_InvalidAccountId_ReturnsFailure()
    {
        // Arrange
        var query = new GetAccountQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await SendRequestAsync<GetAccountQuery, Guid>(query);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Account.NotFound", result.Error.Code);
    }

    private async Task<Result<TResponse>> SendRequestAsync<TRequest, TResponse>(TRequest request)
    {
        if (request == null) // Ensure the request is not null
            throw new ArgumentNullException(nameof(request));

        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(request); // Send the request (command or query)
        if (result is Result<TResponse> typedResult)
        {
            return typedResult;
        }

        throw new InvalidOperationException("Unexpected result type returned from mediator."); // Ensure a valid result is always returned
    }
}
