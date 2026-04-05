using System.Security.Claims;
using System.Text.Encodings.Web;
using Auth.Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Auth.API.Security;

public sealed class SessionAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISessionRepository sessionRepository,
    IUserRepository userRepository)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "SessionAuth";
    public const string SessionIdKey = "SessionId";
    public const string SessionUserIdKey = "UserId";
    public const string SessionUsernameKey = "Username";
    public const string SessionEmailKey = "Email";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var sessionId = ExtractSessionId();
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return AuthenticateResult.NoResult();
        }

        var session = await sessionRepository.GetByIdAsync(sessionId, Context.RequestAborted);
        if (session is null || session.IsExpired(DateTimeOffset.UtcNow))
        {
            return AuthenticateResult.Fail("Session is invalid or expired.");
        }

        var user = await userRepository.GetByIdAsync(session.UserId, Context.RequestAborted);
        if (user is null)
        {
            return AuthenticateResult.Fail("User not found for session.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username?.Value ?? user.Email.Value),
            new("session_id", session.Id)
        };

        if (!string.IsNullOrWhiteSpace(user.Email.Value))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email.Value));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private string? ExtractSessionId()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization) && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authorization["Bearer ".Length..].Trim();
            if (!string.IsNullOrWhiteSpace(token))
            {
                return token;
            }
        }

        var headerSessionId = Request.Headers["X-Session-Id"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerSessionId))
        {
            return headerSessionId;
        }

        return Context.Session.GetString(SessionIdKey);
    }
}
