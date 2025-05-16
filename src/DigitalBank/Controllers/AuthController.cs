using DigitalBank.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            var loginResult = result.Value!;
            Response.Cookies.Append("authToken", loginResult.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Temporarily disabled, ensure HTTPS in production
                SameSite = SameSiteMode.Strict,
                Expires = loginResult.Expiration
            });

            return Ok(new { message = "Login successful" });
        }

        return Unauthorized(result.Error!.ToProblemDetails(StatusCodes.Status401Unauthorized));
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Clear the cookie by setting an expired date
        Response.Cookies.Append("authToken", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Temporarily disabled, ensure HTTPS in production
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        });

        return Ok(new { message = "Logout successful" });
    }

    [Authorize]
    [HttpGet("status")]
    public IActionResult CheckAuthStatus()
    {
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated)
        {
            return Unauthorized(new { message = "User is not authenticated" });
        }

        return Ok(new { isAuthenticated = true, message = "User is authenticated" });
    }
}
