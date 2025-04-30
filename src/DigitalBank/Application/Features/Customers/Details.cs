using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Customers;

public record GetCustomerQuery(Guid CustomerId) : IRequest<Result<Customer>>;

public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, Result<Customer>>
{
    private readonly DigitalBankDbContext _dbContext;

    public GetCustomerQueryHandler(DigitalBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Customer>> Handle(
        GetCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        return customer is null
            ? Result.Failure<Customer>(Errors.Customer.NotFound)
            : Result.Success(customer);
    }
}
