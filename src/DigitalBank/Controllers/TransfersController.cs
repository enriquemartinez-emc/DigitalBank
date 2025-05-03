using DigitalBank.Application.Features.Transfers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/transfers")]
public class TransfersController : ControllerBase
{
    private readonly ISender _sender;
    public TransfersController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetTransfersByCustomer(
        [FromQuery] Guid? customerId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTransfersByCustomerQuery(customerId), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error!.ToProblemDetails(StatusCodes.Status404NotFound));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransfer(
        CreateTransferCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }
}
