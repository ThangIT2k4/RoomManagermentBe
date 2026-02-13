namespace Identity.Application.Features.Users.GetUsersPaged;

public sealed record UserListItemDto(
    Guid Id,
    string Username,
    string Email,
    string Status);

