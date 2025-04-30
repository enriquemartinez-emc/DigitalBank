using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Customers;

public record CreateCustomerCommand(string FirstName, string LastName, string Email) : IRequest<Result<Guid>>;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
    }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly DigitalBankDbContext _dbContext;

    public CreateCustomerCommandHandler(DigitalBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.Customers.AnyAsync(c => c.Email == request.Email.ToLower(), cancellationToken))
            return Result.Failure<Guid>(Errors.Customer.DuplicateEmail);

        var customerResult = Customer.Create(request.FirstName, request.LastName, request.Email);
        if (!customerResult.IsSuccess)
            return Result.Failure<Guid>(customerResult.Error!);

        var customer = customerResult.Value!;
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(customer.Id);
    }
}
