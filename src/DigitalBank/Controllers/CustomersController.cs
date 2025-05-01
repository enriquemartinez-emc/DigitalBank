using DigitalBank.Application.Features.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCustomersQuery(), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(
        CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCustomer), new { customerId = result.Value }, result.Value)
            : BadRequest(result.Error!.ToProblemDetails(StatusCodes.Status400BadRequest));
    }

    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetCustomer(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCustomerQuery(customerId), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error!.ToProblemDetails(StatusCodes.Status404NotFound));
    }
}
