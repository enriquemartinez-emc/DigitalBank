using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Accounts;

public record GetAccountsByCustomerQuery(Guid CustomerId) : IRequest<Result<IEnumerable<Account>>>;

public class GetAccountsByCustomerQueryHandler : IRequestHandler<GetAccountsByCustomerQuery, Result<IEnumerable<Account>>>
{
    private readonly DigitalBankDbContext _dbContext;

    public GetAccountsByCustomerQueryHandler(DigitalBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<Account>>> Handle(
        GetAccountsByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var customerExists = await _dbContext.Customers
            .AnyAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (!customerExists)
            return Result.Failure<IEnumerable<Account>>(Errors.Customer.NotFound);

        var accounts = await _dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.CustomerId == request.CustomerId)
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<Account>>(accounts);
    }
}
