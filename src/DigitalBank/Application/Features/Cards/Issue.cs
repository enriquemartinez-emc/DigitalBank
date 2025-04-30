using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Cards;

public record IssueCardCommand(Guid AccountId, CardType CardType, string CardHolderName) : IRequest<Result<Guid>>;

public class IssueCardCommandValidator : AbstractValidator<IssueCardCommand>
{
    public IssueCardCommandValidator()
    {
        RuleFor(c => c.AccountId).NotEmpty();
        RuleFor(c => c.CardType).IsInEnum();
        RuleFor(c => c.CardHolderName).NotEmpty().MaximumLength(100);
    }
}

public class IssueCardCommandHandler : IRequestHandler<IssueCardCommand, Result<Guid>>
{
    private readonly DigitalBankDbContext _dbContext;

    public IssueCardCommandHandler(DigitalBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(IssueCardCommand request, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

        if (account is null)
            return Result.Failure<Guid>(Errors.Account.NotFound);

        var cardCount = await _dbContext.Cards
            .CountAsync(c => c.AccountId == request.AccountId, cancellationToken);

        var customerFullName = $"{account.Customer.FirstName} {account.Customer.LastName}";
        var cardResult = Card.Create(
            request.AccountId,
            request.CardType,
            request.CardHolderName,
            customerFullName,
            cardCount);

        if (!cardResult.IsSuccess)
            return Result.Failure<Guid>(cardResult.Error!);

        var card = cardResult.Value!;
        _dbContext.Cards.Add(card);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(card.Id);
    }
}
