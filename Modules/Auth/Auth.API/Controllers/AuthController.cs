using System.Security.Claims;
using Auth.API.Common;
using Auth.API.Requests;
using Auth.API.Security;
using Auth.Application.Dtos;
using Auth.Application.Features.Auth.ChangePassword;
using Auth.Application.Features.Auth.ForgotPassword;
using Auth.Application.Features.Auth.Login;
using Auth.Application.Features.Auth.Logout;
using Auth.Application.Features.Register;
using Auth.Application.Features.Auth.ResendOtp;
using Auth.Application.Features.Auth.ResetPassword;
using Auth.Application.Features.Auth.SendOtp;
using Auth.Application.Features.Auth.Users.GetUserById;
using Auth.Application.Features.Auth.VerifyEmail;
using Auth.Application.Features.Auth.VerifyOtp;
using Auth.Application.Features.GetCurrentUser;
using RoomManagerment.Shared.Common;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomManagerment.Shared.Extensions;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAppSender sender) : ControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting("RegisterPolicy")]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<RegisterResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<RegisterResult>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RegisterResult>>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterCommand(request.Email, request.Password, request.FullName, request.Username, request.Phone),
            cancellationToken);
        return this.ToApiCreatedAtActionResult(result, nameof(GetCurrentUser), new { });
    }

    [AllowAnonymous]
    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResult>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResult>>> Login([FromBody] LoginApiRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginCommand(
                request.Login,
                request.Password,
                HttpContext.GetClientIpAddress(),
                HttpContext.GetClientUserAgent(),
                request.RememberMe),
            cancellationToken);

        if (result.IsFailure)
        {
            return this.ToApiActionResult(result);
        }

        if (result.Value is null)
        {
            return this.ToApiActionResult(
                Result<LoginResult>.Failure(Error.Unauthorized("Auth.LoginFailed", "Đăng nhập thất bại.")));
        }

        PersistHybridSession(result.Value);
        return Ok(ApiResponse<LoginResult>.Succeed(result.Value));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<AuthLogoutResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthLogoutResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthLogoutResponse>>> Logout(CancellationToken cancellationToken)
    {
        var sessionId = ResolveSessionId();
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return this.ApiUnauthorized<AuthLogoutResponse>("Session is required.");
        }

        var result = await sender.Send(new LogoutCommand(sessionId), cancellationToken);
        if (result.IsSuccess)
        {
            HttpContext.Session.Clear();
        }

        return this.ToApiVoidActionResult<AuthLogoutResponse>(result);
    }

    [Authorize]
    [HttpGet("me")]
    [HttpGet("current-user")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var sessionId = ResolveSessionId();
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return this.ApiUnauthorized<UserDto>("Session is required.");
        }

        var result = await sender.Send(new GetCurrentUserQuery(sessionId), cancellationToken);
        return this.ToApiActionResult(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse<AuthChangePasswordResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthChangePasswordResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthChangePasswordResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthChangePasswordResponse>>> ChangePassword([FromBody] ChangePasswordApiRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<AuthChangePasswordResponse>("User identity is missing.");
        }

        var sessionId = ResolveSessionId();
        var result = await sender.Send(
            new ChangePasswordCommand(userId.Value, request.CurrentPassword, request.NewPassword, sessionId),
            cancellationToken);
        return this.ToApiVoidActionResult<AuthChangePasswordResponse>(result);
    }

    [AllowAnonymous]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<AuthForgotPasswordResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthForgotPasswordResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthForgotPasswordResponse>>> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        return this.ToApiVoidActionResult<AuthForgotPasswordResponse>(result);
    }

    [AllowAnonymous]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<AuthResetPasswordResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResetPasswordResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResetPasswordResponse>>> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ResetPasswordCommand(request.Email, request.OtpCode, request.NewPassword),
            cancellationToken);
        return this.ToApiVoidActionResult<AuthResetPasswordResponse>(result);
    }

    [Authorize]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("send-otp")]
    [ProducesResponseType(typeof(ApiResponse<AuthSendOtpResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthSendOtpResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthSendOtpResponse>>> SendOtp([FromBody] SendOtpRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<AuthSendOtpResponse>("User identity is missing.");
        }

        var userResult = await sender.Send(new GetUserByIdQuery(userId.Value), cancellationToken);
        if (userResult.IsFailure || userResult.Value is null)
        {
            return this.ApiUnauthorized<AuthSendOtpResponse>("User not found.");
        }

        if (!string.Equals(userResult.Value.Email, request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return this.ApiBadRequest<AuthSendOtpResponse>("Email does not match the authenticated account.");
        }

        var result = await sender.Send(
            new SendOtpCommand(request.Email, request.Purpose, userId.Value),
            cancellationToken);
        return this.ToApiVoidActionResult<AuthSendOtpResponse>(result);
    }

    [AllowAnonymous]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(ApiResponse<AuthVerifyOtpResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthVerifyOtpResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthVerifyOtpResponse>>> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new VerifyOtpCommand(request.Email, request.Purpose, request.OtpCode),
            cancellationToken);
        return this.ToApiVoidActionResult<AuthVerifyOtpResponse>(result);
    }

    [Authorize]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("resend-otp")]
    [ProducesResponseType(typeof(ApiResponse<AuthResendOtpResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResendOtpResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResendOtpResponse>>> ResendOtp([FromBody] ResendOtpRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<AuthResendOtpResponse>("User identity is missing.");
        }

        var userResult = await sender.Send(new GetUserByIdQuery(userId.Value), cancellationToken);
        if (userResult.IsFailure || userResult.Value is null)
        {
            return this.ApiUnauthorized<AuthResendOtpResponse>("User not found.");
        }

        if (!string.Equals(userResult.Value.Email, request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return this.ApiBadRequest<AuthResendOtpResponse>("Email does not match the authenticated account.");
        }

        var result = await sender.Send(
            new ResendOtpCommand(request.Email, request.Purpose, userId.Value),
            cancellationToken);
        return this.ToApiVoidActionResult<AuthResendOtpResponse>(result);
    }

    [AllowAnonymous]
    [EnableRateLimiting("OtpPolicy")]
    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(ApiResponse<AuthVerifyEmailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthVerifyEmailResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthVerifyEmailResponse>>> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new VerifyEmailCommand(request.Email, request.OtpCode), cancellationToken);
        return this.ToApiVoidActionResult<AuthVerifyEmailResponse>(result);
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
}
