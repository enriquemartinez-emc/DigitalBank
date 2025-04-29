using DigitalBank.Application.Features.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AccountsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }

    [HttpGet("{accountId}/balance")]
    public async Task<IActionResult> GetBalance(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountBalanceQuery(accountId), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }
}
