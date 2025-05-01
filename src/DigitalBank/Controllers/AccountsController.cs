using DigitalBank.Application.Features.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/customers")]
public class AccountsController : ControllerBase
{
    private readonly ISender _sender;
    public AccountsController(ISender sender) => _sender = sender;

    [HttpPost("{customerId}/accounts")]
    public async Task<IActionResult> CreateAccount(
        Guid customerId,
        AccountData data,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateAccountCommand(data, customerId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }

    [HttpGet("{customerId}/accounts/{accountId}/balance")]
    public async Task<IActionResult> GetBalance(
        Guid customerId,
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAccountBalanceQuery(accountId, customerId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error!.ToProblemDetails(StatusCodes.Status404NotFound));
    }

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
}
