using MediatR;
using Microsoft.EntityFrameworkCore;
using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using DigitalBank.Domain.Common.Errors;

namespace DigitalBank.Application.Features.Transfers;

public record GetTransfersByCustomerQuery(Guid? CustomerId) : IRequest<Result<IEnumerable<Transfer>>>;

public class GetTransfersByCustomerQueryHandler : IRequestHandler<GetTransfersByCustomerQuery, Result<IEnumerable<Transfer>>>
{
    private readonly DigitalBankDbContext _dbContext;

    public GetTransfersByCustomerQueryHandler(DigitalBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<Transfer>>> Handle(
        GetTransfersByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Transfers.AsQueryable();

        if (request.CustomerId.HasValue)
        {
            // LEFT JOIN Transfers with Accounts on FromAccountId
            var withFromAccounts = _dbContext.Transfers
                .GroupJoin(_dbContext.Accounts,
                    transfer => transfer.FromAccountId,
                    account => account.Id,
                    (transfer, fromAccounts) => new { transfer, fromAccounts })
                .SelectMany(
                    x => x.fromAccounts.DefaultIfEmpty(),
                    (x, fromAccount) => new { x.transfer, FromAccount = fromAccount });

            // LEFT JOIN with Accounts on ToAccountId
            var withBothAccounts = withFromAccounts
                .GroupJoin(_dbContext.Accounts,
                    x => x.transfer.ToAccountId,
                    account => account.Id,
                    (x, toAccounts) => new { x.transfer, x.FromAccount, toAccounts })
                .SelectMany(
                    x => x.toAccounts.DefaultIfEmpty(),
                    (x, toAccount) => new { x.transfer, x.FromAccount, ToAccount = toAccount });

            // Filter where customer owns either FromAccount or ToAccount
            query = withBothAccounts
                .Where(x => (x.FromAccount != null && x.FromAccount.CustomerId == request.CustomerId.Value) ||
                            (x.ToAccount != null && x.ToAccount.CustomerId == request.CustomerId.Value))
                .Select(x => x.transfer);
        }

        var transfers = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        if (request.CustomerId.HasValue && transfers.Count == 0)
        {
            return Result.Failure<IEnumerable<Transfer>>(
                Errors.Transfer.NotFound);
        }

        return Result.Success<IEnumerable<Transfer>>(transfers);
    }
}