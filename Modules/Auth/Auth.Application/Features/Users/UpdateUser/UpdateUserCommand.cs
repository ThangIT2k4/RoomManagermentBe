using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string? Email,
    string? Username,
    string? Phone,
    short? Status) : IAppRequest<Result<UserDto>>;
