using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using FluentValidation;
using MediatR;

namespace DigitalBank.Application.Features.Transfers;

public record CreateTransferCommand(Guid FromAccountId, Guid ToAccountId, decimal Amount) : IRequest<Result<Guid>>;

public class CreateTransferCommandValidator : AbstractValidator<CreateTransferCommand>
{
    public CreateTransferCommandValidator()
    {
        RuleFor(x => x.FromAccountId)
            .NotEmpty();
        RuleFor(x => x.ToAccountId)
            .NotEmpty()
            .NotEqual(x => x.FromAccountId);
        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}

public class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand, Result<Guid>>
{
    private readonly DigitalBankDbContext _context;
    public CreateTransferCommandHandler(DigitalBankDbContext context) => _context = context;

    public async Task<Result<Guid>> Handle(
        CreateTransferCommand request,
        CancellationToken cancellationToken)
    {
        var fromAccount = await _context.Accounts.FindAsync([request.FromAccountId], cancellationToken);
        var toAccount = await _context.Accounts.FindAsync([request.ToAccountId], cancellationToken);

        if (fromAccount is null || toAccount is null)
            return Result.Failure<Guid>(Errors.Account.NotFound);

        var withdrawalResult = fromAccount.UpdateBalance(request.Amount, TransactionType.Withdrawal);
        if (!withdrawalResult.IsSuccess)
            return Result.Failure<Guid>(withdrawalResult.Error!);

        var depositResult = toAccount.UpdateBalance(request.Amount, TransactionType.Deposit);
        if (!depositResult.IsSuccess)
            return Result.Failure<Guid>(depositResult.Error!);

        var transfer = new Transfer
        {
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            Amount = request.Amount
        };

        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(transfer.Id);
    }
}
