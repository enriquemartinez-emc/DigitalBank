using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using FluentValidation;
using MediatR;

namespace DigitalBank.Application.Features.Accounts;

public record CreateAccountCommand(string AccountNumber, Guid CustomerId, decimal InitialBalance) : IRequest<Result<Guid>>;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .Length(10, 20);
        RuleFor(x => x.CustomerId)
            .NotEmpty();
        RuleFor(x => x.InitialBalance)
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
            return Result.Failure<Guid>(Errors.Account.NotFound());

        var accountResult = Account.Create(request.AccountNumber, request.CustomerId, request.InitialBalance);
        if (!accountResult.IsSuccess)
            return Result.Failure<Guid>(accountResult.Error!);

        _context.Accounts.Add(accountResult.Value!);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(accountResult.Value!.Id);
    }
}
