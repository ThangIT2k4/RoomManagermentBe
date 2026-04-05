using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string? Email,
    string? Username,
    string? Phone,
    short? Status) : IRequest<Result<UserDto>>;
