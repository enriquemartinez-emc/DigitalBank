using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Application.Features.Cards;

public record GetCardQuery(Guid CardId) : IRequest<Result<Card>>;

public class GetCardQueryHandler : IRequestHandler<GetCardQuery, Result<Card>>
{
    private readonly DigitalBankDbContext _dbContext;

    public GetCardQueryHandler(DigitalBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Card>> Handle(GetCardQuery request, CancellationToken cancellationToken)
    {
        var card = await _dbContext.Cards
            .FirstOrDefaultAsync(c => c.Id == request.CardId, cancellationToken);

        return card is null
            ? Result.Failure<Card>(Errors.Card.NotFound)
            : Result.Success(card);
    }
}
