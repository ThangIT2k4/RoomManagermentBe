namespace Identity.Application.Features.Users.Commands.RegisterUser;

public sealed record RegisterUserResult(
    Guid Id,
    string Username,
    string Email,
    string Status
);

