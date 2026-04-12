using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.GetCurrentUser;

public sealed record GetCurrentUserQuery(string SessionId) : IAppRequest<Result<UserDto>>;
