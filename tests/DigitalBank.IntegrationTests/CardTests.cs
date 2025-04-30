using DigitalBank.Application.Features.Cards;
using DigitalBank.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalBank.IntegrationTests;

public class CardIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task IssueCard_ValidInput_IssuesCardAndReturnsId()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "john.doe@example.com").Value!;
        var account = Account.Create("12345678901234", customer.Id, 1000m).Value!;

        using (var db = await CreateDbContextAsync())
        {
            db.Customers.Add(customer);
            db.Accounts.Add(account);
            await db.SaveChangesAsync();
        }

        var command = new IssueCardCommand(
            AccountId: account.Id,
            CardType: CardType.Debit,
            CardHolderName: "John Doe");

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        using var dbContext = await CreateDbContextAsync();
        var card = await dbContext.Cards.FirstOrDefaultAsync(c => c.Id == result.Value);

        Assert.NotNull(card);
        Assert.Equal(command.AccountId, card.AccountId);
        Assert.Equal(command.CardType, card.CardType);
        Assert.Equal(command.CardHolderName, card.CardHolderName);
    }

    [Fact]
    public async Task IssueCard_TooManyCards_ReturnsFailure()
    {
        var customer = Customer.Create("John", "Doe", "john.doe@example.com").Value!;
        var account = Account.Create("12345678901234", customer.Id, 1000m).Value!;

        using (var dbContext = await CreateDbContextAsync())
        {
            dbContext.Customers.Add(customer);
            dbContext.Accounts.Add(account);
            for (int i = 0; i < 3; i++)
            {
                var cardResult = Card.Create(account.Id, CardType.Debit, "John Doe", "John Doe", i);
                if (cardResult.IsSuccess) dbContext.Cards.Add(cardResult.Value!);
            }
            await dbContext.SaveChangesAsync();
        }

        var command = new IssueCardCommand(account.Id, CardType.Debit, "John Doe");

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Card.TooManyCards", result.Error!.Code);
    }

    [Fact]
    public async Task GetCard_ValidId_ReturnsCard()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "john.doe@example.com").Value!;
        var account = Account.Create("12345678901234", customer.Id, 1000m).Value!;
        var card = Card.Create(account.Id, CardType.Debit, "John Doe", "John Doe", 0).Value!;

        using (var dbContext = await CreateDbContextAsync())
        {
            dbContext.Customers.Add(customer);
            dbContext.Accounts.Add(account);
            dbContext.Cards.Add(card);
            await dbContext.SaveChangesAsync();
        }

        var query = new GetCardQuery(card.Id);

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(card.Id, result.Value!.Id);
        Assert.Equal(card.CardNumber, result.Value.CardNumber);
    }

    [Fact]
    public async Task GetCard_InvalidId_ReturnsFailure()
    {
        // Arrange
        var query = new GetCardQuery(Guid.NewGuid());

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(query);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Card.NotFound", result.Error!.Code);
    }
}
