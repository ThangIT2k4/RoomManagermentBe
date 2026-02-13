namespace Identity.Application.Features.Auth.CreateRefreshToken;

public sealed record CreateRefreshTokenCommand(
    Guid UserId,
    string Token,
    DateTime ExpiresAt);

