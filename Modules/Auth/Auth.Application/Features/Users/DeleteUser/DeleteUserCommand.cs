using Auth.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId, Guid DeletedBy) : IAppRequest<Result>;
