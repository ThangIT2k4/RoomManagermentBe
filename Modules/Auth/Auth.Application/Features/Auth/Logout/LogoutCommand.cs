using Auth.Application.Common;
using MediatR;

namespace Auth.Application.Features.Auth.Logout;

public sealed record LogoutCommand(string SessionId) : IRequest<Result>;
