using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Auth.RevokeRefreshToken;

public sealed record RevokeRefreshTokenCommand(
    string Token) : IRequest<Result>;

