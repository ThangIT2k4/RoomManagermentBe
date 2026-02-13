namespace Identity.Application.Features.Users.GetUsersPaged;

public sealed record GetUsersPagedQuery(
    int Page,
    int PageSize);