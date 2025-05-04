using DigitalBank.Application.Features.Accounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly ISender _sender;
    public AccountsController(ISender sender) => _sender = sender;

    [HttpGet("{customerId}/accounts")]
    public async Task<IActionResult> GetAccountsByCustomer(
    Guid customerId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAccountsByCustomerQuery(customerId), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error!.ToProblemDetails(StatusCodes.Status404NotFound));
    }

    [HttpPost("{customerId}/accounts")]
    public async Task<IActionResult> CreateAccount(
        Guid customerId,
        [FromBody] AccountData data,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateAccountCommand(data, customerId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }

    [HttpGet("{customerId}/accounts/{accountId}")]
    public async Task<IActionResult> GetAccount(
        Guid customerId,
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAccountQuery(accountId, customerId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error!.ToProblemDetails(StatusCodes.Status404NotFound));
    }

    [HttpGet("accounts/batch")]
    public async Task<IActionResult> GetAccountsByIds(
        [FromQuery] Guid[] accountIds,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAccountsByIdsQuery(accountIds), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error!.ToProblemDetails(StatusCodes.Status404NotFound));
    }
}
