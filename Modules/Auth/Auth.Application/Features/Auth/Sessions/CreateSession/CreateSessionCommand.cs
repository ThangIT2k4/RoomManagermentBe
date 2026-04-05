using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Sessions.CreateSession;

public sealed record CreateSessionCommand(
    Guid UserId,
    string? IpAddress,
    string? UserAgent,
    bool RememberMe) : IRequest<Result<SessionDto>>;
