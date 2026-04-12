using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ChangeUserStatus;

public sealed record ChangeUserStatusCommand(Guid UserId, short Status) : IAppRequest<Result<UserDto>>;
