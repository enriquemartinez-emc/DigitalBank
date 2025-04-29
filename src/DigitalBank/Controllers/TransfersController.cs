using DigitalBank.Application.Features.Transfers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly IMediator _mediator;
    public TransfersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateTransfer(
        CreateTransferCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }
}
