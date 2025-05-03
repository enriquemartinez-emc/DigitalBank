using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Accounts;

public record GetAccountQuery(Guid AccountId, Guid CustomerId) : IRequest<Result<Account>>;

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, Result<Account>>
{
    private readonly DigitalBankDbContext _context;
    public GetAccountQueryHandler(DigitalBankDbContext context) => _context = context;

    public async Task<Result<Account>> Handle(
        GetAccountQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync([request.CustomerId], cancellationToken);
        if (customer is null)
            return Result.Failure<Account>(Errors.Customer.NotFound);

        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.CustomerId == request.CustomerId, cancellationToken);

        if (account is null)
            return Result.Failure<Account>(Errors.Account.NotFound);

        return Result.Success(account);
    }
}
