using MediatR;
using Microsoft.EntityFrameworkCore;
using DigitalBank.Domain.Common;
using DigitalBank.Infrastructure.Persistence;
using DigitalBank.Domain.Common.Errors;

namespace DigitalBank.Application.Features.Transfers;

public record GetTransfersByCustomerQuery(Guid? CustomerId) : IRequest<Result<IEnumerable<TransferResponse>>>;

public class GetTransfersByCustomerQueryHandler : IRequestHandler<GetTransfersByCustomerQuery, Result<IEnumerable<TransferResponse>>>
{
    private readonly DigitalBankDbContext _dbContext;

    public GetTransfersByCustomerQueryHandler(DigitalBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<TransferResponse>>> Handle(
        GetTransfersByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        if (!request.CustomerId.HasValue)
        {
            return Result.Failure<IEnumerable<TransferResponse>>(Errors.Customer.NotFound);
        }

        var customerAccountIds = await _dbContext.Accounts
            .Where(a => a.CustomerId == request.CustomerId.Value)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        var response = await _dbContext.Transfers
            .AsNoTracking()
            .Where(t =>
                customerAccountIds.Contains(t.FromAccountId) ||
                customerAccountIds.Contains(t.ToAccountId))
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TransferResponse(
                t.Id,
                t.Amount,
                t.FromAccount.AccountNumber,
                t.ToAccount.AccountNumber,
                t.FromAccount.Customer != null ? t.FromAccount.Customer.FullName : string.Empty,
                t.ToAccount.Customer != null ? t.ToAccount.Customer.FullName : string.Empty,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        if (response.Count == 0)
        {
            return Result.Failure<IEnumerable<TransferResponse>>(Errors.Transfer.NotFound);
        }

        return Result.Success<IEnumerable<TransferResponse>>(response);
    }
}