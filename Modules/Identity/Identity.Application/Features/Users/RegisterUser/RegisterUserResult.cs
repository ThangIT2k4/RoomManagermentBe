namespace Identity.Application.Features.Users.RegisterUser;

public sealed record RegisterUserResult(
    Guid Id,
    string Username,
    string Email,
    string Status
);

