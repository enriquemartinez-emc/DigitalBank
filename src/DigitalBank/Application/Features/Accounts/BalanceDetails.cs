using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;
using DigitalBank.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Accounts;

public record GetAccountBalanceQuery(Guid AccountId, Guid CustomerId) : IRequest<Result<decimal>>;

public class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, Result<decimal>>
{
    private readonly DigitalBankDbContext _context;
    public GetAccountBalanceQueryHandler(DigitalBankDbContext context) => _context = context;

    public async Task<Result<decimal>> Handle(
        GetAccountBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync([request.CustomerId], cancellationToken);
        if (customer is null)
            return Result.Failure<decimal>(Errors.Customer.NotFound);

        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.CustomerId == request.CustomerId, cancellationToken);

        if (account is null)
            return Result.Failure<decimal>(Errors.Account.NotFound);

        return Result.Success(account.Balance);
    }
}
