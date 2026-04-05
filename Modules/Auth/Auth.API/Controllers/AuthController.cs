using System.Security.Claims;
using Auth.API.Common;
using Auth.API.Requests;
using Auth.API.Security;
using Auth.Application.Dtos;
using Auth.Application.Features.Auth.ChangePassword;
using Auth.Application.Features.Auth.ForgotPassword;
using Auth.Application.Features.Auth.Login;
using Auth.Application.Features.Auth.Logout;
using Auth.Application.Features.Auth.Register;
using Auth.Application.Features.Auth.ResendOtp;
using Auth.Application.Features.Auth.ResetPassword;
using Auth.Application.Features.Auth.SendOtp;
using Auth.Application.Features.Auth.Users.GetUserById;
using Auth.Application.Features.Auth.VerifyEmail;
using Auth.Application.Features.Auth.VerifyOtp;
using Auth.Application.Features.GetCurrentUser;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IMediator mediator,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginApiRequest> loginValidator,
    IValidator<ChangePasswordRequest> changePasswordValidator,
    IValidator<ForgotPasswordRequest> forgotPasswordValidator,
    IValidator<ResetPasswordRequest> resetPasswordValidator,
    IValidator<SendOtpRequest> sendOtpValidator,
    IValidator<VerifyOtpRequest> verifyOtpValidator,
    IValidator<ResendOtpRequest> resendOtpValidator,
    IValidator<VerifyEmailRequest> verifyEmailValidator) : ControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting("RegisterPolicy")]
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterResult>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var validation = await registerValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await mediator.Send(
            new RegisterCommand(request.Email, request.Password, request.FullName, request.Username, request.Phone),
            cancellationToken);
        return result.ToCreatedAtActionResult(nameof(GetCurrentUser), new { });
    }

    [AllowAnonymous]
    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginApiRequest request, CancellationToken cancellationToken)
    {
        var validation = await loginValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await mediator.Send(
            new LoginCommand(
                request.Login,
                request.Password,
                HttpContext.GetClientIpAddress(),
                HttpContext.GetClientUserAgent(),
                request.RememberMe),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return result.ToActionResult();
        }

        PersistHybridSession(result.Value);
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        var sessionId = ResolveSessionId();
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Unauthorized(new { message = "Session is required." });
        }

        var result = await mediator.Send(new LogoutCommand(sessionId), cancellationToken);
        if (result.IsSuccess)
        {
            HttpContext.Session.Clear();
        }

        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("me")]
    [HttpGet("current-user")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var sessionId = ResolveSessionId();
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Unauthorized(new { message = "Session is required." });
        }

        var result = await mediator.Send(new GetCurrentUserQuery(sessionId), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordApiRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var sessionId = ResolveSessionId();
        var command = new ChangePasswordRequest(userId.Value, request.CurrentPassword, request.NewPassword, sessionId);
        var validation = await changePasswordValidator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await mediator.Send(
            new ChangePasswordCommand(userId.Value, request.CurrentPassword, request.NewPassword, sessionId),
            cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var validation = await forgotPasswordValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await mediator.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var validation = await resetPasswordValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await mediator.Send(
            new ResetPasswordCommand(request.Email, request.OtpCode, request.NewPassword),
            cancellationToken);
        return result.ToActionResult();
    }

    [Authorize]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("send-otp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SendOtp([FromBody] SendOtpRequest request, CancellationToken cancellationToken)
    {
        var validation = await sendOtpValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var userResult = await mediator.Send(new GetUserByIdQuery(userId.Value), cancellationToken);
        if (userResult.IsFailure || userResult.Value is null)
        {
            return Unauthorized(new { message = "User not found." });
        }

        if (!string.Equals(userResult.Value.Email, request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Email does not match the authenticated account." });
        }

        var result = await mediator.Send(
            new SendOtpCommand(request.Email, request.Purpose, userId.Value),
            cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("verify-otp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        var validation = await verifyOtpValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await mediator.Send(
            new VerifyOtpCommand(request.Email, request.Purpose, request.OtpCode),
            cancellationToken);
        return result.ToActionResult();
    }

    [Authorize]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("resend-otp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResendOtp([FromBody] ResendOtpRequest request, CancellationToken cancellationToken)
    {
        var validation = await resendOtpValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var userResult = await mediator.Send(new GetUserByIdQuery(userId.Value), cancellationToken);
        if (userResult.IsFailure || userResult.Value is null)
        {
            return Unauthorized(new { message = "User not found." });
        }

        if (!string.Equals(userResult.Value.Email, request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Email does not match the authenticated account." });
        }

        var result = await mediator.Send(
            new ResendOtpCommand(request.Email, request.Purpose, userId.Value),
            cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var validation = await verifyEmailValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await mediator.Send(new VerifyEmailCommand(request.Email, request.OtpCode), cancellationToken);
        return result.ToActionResult();
    }

    private void PersistHybridSession(LoginResult loginResult)
    {
        HttpContext.Session.SetString(SessionAuthenticationHandler.SessionIdKey, loginResult.Session.Id);
        HttpContext.Session.SetString(SessionAuthenticationHandler.SessionUserIdKey, loginResult.User.Id.ToString());
        HttpContext.Session.SetString(SessionAuthenticationHandler.SessionEmailKey, loginResult.User.Email);
        HttpContext.Session.SetString(SessionAuthenticationHandler.SessionUsernameKey, loginResult.User.Username ?? loginResult.User.Email);
    }

    private string? ResolveSessionId()
    {
        var claimSessionId = User.FindFirstValue("session_id");
        if (!string.IsNullOrWhiteSpace(claimSessionId))
        {
            return claimSessionId;
        }

        return HttpContext.Session.GetString(SessionAuthenticationHandler.SessionIdKey);
    }

    private Guid? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(raw, out var userId))
        {
            return userId;
        }

        raw = HttpContext.Session.GetString(SessionAuthenticationHandler.SessionUserIdKey);
        return Guid.TryParse(raw, out userId) ? userId : null;
    }

    private static object ToValidationErrorPayload(ValidationResult validationResult)
    {
        return new
        {
            message = "Validation failed",
            errors = validationResult.Errors.Select(error => new
            {
                field = error.PropertyName,
                error = error.ErrorMessage
            })
        };
    }
}
