using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Auth.CreateRefreshToken;

public sealed record CreateRefreshTokenCommand(
    Guid UserId,
    string Token,
    DateTime ExpiresAt) : IRequest<Result>;

