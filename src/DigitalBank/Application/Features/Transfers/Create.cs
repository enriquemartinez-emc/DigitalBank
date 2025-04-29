using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using MediatR;

namespace DigitalBank.Application.Features.Transfers;

public record CreateTransferCommand(Guid FromAccountId, Guid ToAccountId, decimal Amount) : IRequest<Result<Guid>>;

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
            return Result<Guid>.Failure<Guid>(Errors.Account.NotFound());

        var withdrawalResult = fromAccount.UpdateBalance(request.Amount, TransactionType.Withdrawal);
        if (!withdrawalResult.IsSuccess)
            return Result<Guid>.Failure<Guid>(withdrawalResult.Error!);

        var depositResult = toAccount.UpdateBalance(request.Amount, TransactionType.Deposit);
        if (!depositResult.IsSuccess)
            return Result<Guid>.Failure<Guid>(depositResult.Error!);

        var transfer = new Transfer
        {
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            Amount = request.Amount
        };

        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(transfer.Id);
    }
}
