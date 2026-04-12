using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Login,
    string Password,
    string? IpAddress,
    string? UserAgent,
    bool RememberMe) : IAppRequest<Result<LoginResult>>;
