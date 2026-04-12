using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Sessions.CreateSession;

public sealed record CreateSessionCommand(
    Guid UserId,
    string? IpAddress,
    string? UserAgent,
    bool RememberMe) : IAppRequest<Result<SessionDto>>;
