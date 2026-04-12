using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IAppRequest<Result<UserDto>>;
