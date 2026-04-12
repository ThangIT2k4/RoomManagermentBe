using Auth.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Logout;

public sealed record LogoutCommand(string SessionId) : IAppRequest<Result>;
