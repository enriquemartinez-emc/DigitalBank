using DigitalBank.Domain.Common;
using DigitalBank.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Accounts;

public record GetAccountBalanceQuery(Guid AccountId) : IRequest<Result<decimal>>;

public class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, Result<decimal>>
{
    private readonly DigitalBankDbContext _context;
    public GetAccountBalanceQueryHandler(DigitalBankDbContext context) => _context = context;

    public async Task<Result<decimal>> Handle(
        GetAccountBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

        if (account is null)
            return Result.Failure<decimal>(Errors.Account.NotFound);

        return Result.Success(account.Balance);
    }
}
