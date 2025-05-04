using DigitalBank.Application.Features.Cards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/cards")]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly ISender _sender;

    public CardsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> IssueCard(
        IssueCardCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCard), new { cardId = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{cardId}")]
    public async Task<IActionResult> GetCard(Guid cardId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCardQuery(cardId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
}
