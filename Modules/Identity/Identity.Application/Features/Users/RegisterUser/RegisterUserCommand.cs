namespace Identity.Application.Features.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Username,
    string Email,
    string Password
);

