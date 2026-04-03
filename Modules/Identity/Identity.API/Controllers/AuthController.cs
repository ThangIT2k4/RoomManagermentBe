using Identity.API.Common;
using Identity.API.Requests;
using Identity.Application.Features.Auth.Login;
using Identity.Application.Features.Auth.Register;
using FluentValidation;
using FluentValidation.Results;
using Identity.Application.Features.Auth.ChangePassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IMediator mediator,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator) : ControllerBase
{
    private static object ToValidationErrorPayload(ValidationResult validationResult)
        => new
        {
            message = "Validation failed",
            errors = validationResult.Errors
                .Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
        };

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register(
        [FromBody] RegisterRequest request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validationResult));
        }

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

    [AllowAnonymous]
    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Login(
        [FromBody] LoginRequest request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validationResult));
        }

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

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "Đăng xuất thành công" });
    }

    [Authorize]
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

    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return BadRequest(new { message = "Mật khẩu mới và xác nhận mật khẩu không khớp" });
        }
        
        var userIdRaw = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrWhiteSpace(userIdRaw) || !Guid.TryParse(userIdRaw, out var userId))
        {
            return Unauthorized(new { message = "Bạn chưa đăng nhập" });
        }

        var command = new ChangePasswordCommand()
        {
            UserId = userId,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };
        
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess 
            ? Ok(new { message = "Đổi mật khẩu thành công" }) 
            : BadRequest(new { message = result.Error });
    }
    
}
