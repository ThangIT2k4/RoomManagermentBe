using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Login,
    string Password,
    string? IpAddress,
    string? UserAgent,
    bool RememberMe) : IRequest<Result<LoginResult>>;
