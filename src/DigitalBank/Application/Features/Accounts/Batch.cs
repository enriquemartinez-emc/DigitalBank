using MediatR;
using Microsoft.EntityFrameworkCore;
using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using DigitalBank.Domain.Common.Errors;

namespace DigitalBank.Application.Features.Accounts;

public record GetAccountsByIdsQuery(Guid[] AccountIds) : IRequest<Result<IEnumerable<Account>>>;

public class GetAccountsByIdsQueryHandler : IRequestHandler<GetAccountsByIdsQuery, Result<IEnumerable<Account>>>
{
    private readonly DigitalBankDbContext _dbContext;

    public GetAccountsByIdsQueryHandler(DigitalBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<Account>>> Handle(
        GetAccountsByIdsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await _dbContext.Accounts
            .Where(a => request.AccountIds.Contains(a.Id))
            .ToListAsync(cancellationToken);

        if (accounts.Count == 0)
        {
            return Result.Failure<IEnumerable<Account>>(Errors.Account.NotFound);
        }

        return Result.Success<IEnumerable<Account>>(accounts);
    }
}