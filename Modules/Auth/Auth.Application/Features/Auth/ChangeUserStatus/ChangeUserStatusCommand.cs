using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.ChangeUserStatus;

public sealed record ChangeUserStatusCommand(Guid UserId, short Status) : IRequest<Result<UserDto>>;
