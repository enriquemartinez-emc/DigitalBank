using DigitalBank.Domain.Common;
using DigitalBank.Domain.Common.Errors;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using FluentValidation;
using MediatR;

namespace DigitalBank.Application.Features.Accounts;

public record AccountData(string AccountNumber, decimal InitialBalance);
public record CreateAccountCommand(AccountData AccountData, Guid CustomerId) : IRequest<Result<Guid>>;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");
        RuleFor(x => x.AccountData.AccountNumber)
            .NotEmpty()
            .Length(10, 20);
        RuleFor(x => x.CustomerId)
            .NotEmpty();
        RuleFor(x => x.AccountData.InitialBalance)
            .GreaterThanOrEqualTo(0);
    }
}

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<Guid>>
{
    private readonly DigitalBankDbContext _context;
    public CreateAccountCommandHandler(DigitalBankDbContext context) => _context = context;

    public async Task<Result<Guid>> Handle(
        CreateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync([request.CustomerId], cancellationToken);
        if (customer is null)
            return Result.Failure<Guid>(Errors.Customer.NotFound);

        var accountResult = Account.Create(
            request.AccountData.AccountNumber,
            request.CustomerId,
            request.AccountData.InitialBalance);

        if (!accountResult.IsSuccess)
            return Result.Failure<Guid>(accountResult.Error!);

        await _context.Accounts.AddAsync(accountResult.Value!);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(accountResult.Value!.Id);
    }
}
