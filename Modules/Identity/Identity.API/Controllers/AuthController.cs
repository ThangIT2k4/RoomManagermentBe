using Identity.API.Common;
using Identity.API.Requests;
using Identity.Application.Features.Auth.CreateRefreshToken;
using Identity.Application.Features.Auth.Login;
using Identity.Application.Features.Auth.Register;
using Identity.Application.Features.Auth.RevokeRefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register(
        [FromBody] RegisterRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        var result = await mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        return BadRequest(new { message = result.Error });
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Login(
        [FromBody] LoginRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand
        {
            UsernameOrEmail = request.UsernameOrEmail,
            Password = request.Password,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        var result = await mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            var loginData = result.Value!;
            
            // Store session data
            HttpContext.Session.SetString("UserId", loginData.UserId.ToString());
            HttpContext.Session.SetString("Username", loginData.Username);
            HttpContext.Session.SetString("Email", loginData.Email);
            HttpContext.Session.SetString("LoginTime", DateTime.UtcNow.ToString("O"));
            
            return Ok(new 
            { 
                message = "Đăng nhập thành công",
                data = loginData
            });
        }

        return Unauthorized(new { message = result.Error });
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "Đăng xuất thành công" });
    }

    [HttpGet("current-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult GetCurrentUser()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var username = HttpContext.Session.GetString("Username");
        var email = HttpContext.Session.GetString("Email");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Bạn chưa đăng nhập" });
        }

        return Ok(new 
        { 
            userId,
            username,
            email
        });
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CreateRefreshToken(
        [FromBody] CreateRefreshTokenCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Created(string.Empty, null);
        }

        return result.ToActionResult();
    }

    [HttpPost("refresh-token/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RevokeRefreshToken(
        [FromBody] RevokeRefreshTokenRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new RevokeRefreshTokenCommand(request.Token);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
