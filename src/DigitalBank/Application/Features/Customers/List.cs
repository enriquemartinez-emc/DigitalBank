using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Customers;

public record GetCustomersQuery : IRequest<Result<IEnumerable<Customer>>>;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<IEnumerable<Customer>>>
{
    private readonly DigitalBankDbContext _dbContext;

    public GetCustomersQueryHandler(DigitalBankDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<IEnumerable<Customer>>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await _dbContext.Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<Customer>>(customers);
    }
}
