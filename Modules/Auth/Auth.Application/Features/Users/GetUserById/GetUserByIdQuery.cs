using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;
