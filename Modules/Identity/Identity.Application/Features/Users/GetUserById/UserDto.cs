namespace Identity.Application.Features.Users.GetUserById;

public sealed record UserDto(
    Guid Id,
    string Username,
    string Email,
    string Status);

