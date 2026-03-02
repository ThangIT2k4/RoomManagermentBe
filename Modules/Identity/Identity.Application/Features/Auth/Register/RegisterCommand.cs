using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Auth.Register;

public class RegisterCommand : IRequest<Result<RegisterResult>>
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RegisterResult
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}
