using Auth.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Sessions.LogoutAllSessions;

public sealed record LogoutAllSessionsCommand(Guid UserId) : IAppRequest<Result>;
