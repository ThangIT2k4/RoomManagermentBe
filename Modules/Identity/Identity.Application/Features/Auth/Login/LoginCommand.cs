using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Auth.Login;

public class LoginCommand : IRequest<Result<LoginResult>>
{
    public required string UsernameOrEmail { get; set; }
    public required string Password { get; set; }
}