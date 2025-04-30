using DigitalBank.Application.Features.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly ISender _sender;
    public AccountsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }

    [HttpGet("{accountId}/balance")]
    public async Task<IActionResult> GetBalance(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAccountBalanceQuery(accountId), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }
}
