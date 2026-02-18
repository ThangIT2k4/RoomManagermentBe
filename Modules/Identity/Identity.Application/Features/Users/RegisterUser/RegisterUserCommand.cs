using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Username,
    string Email,
    string Password
) : IRequest<Result<RegisterUserResult>>;

