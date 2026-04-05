using Auth.Application.Common;
using MediatR;

namespace Auth.Application.Features.Auth.Sessions.LogoutAllSessions;

public sealed record LogoutAllSessionsCommand(Guid UserId) : IRequest<Result>;
