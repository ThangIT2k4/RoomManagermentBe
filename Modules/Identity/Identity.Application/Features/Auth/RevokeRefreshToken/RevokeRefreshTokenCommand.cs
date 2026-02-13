namespace Identity.Application.Features.Auth.RevokeRefreshToken;

public sealed record RevokeRefreshTokenCommand(
    string Token);

