using Identity.Application.Common;
using Identity.Domain.Common;
using MediatR;

namespace Identity.Application.Features.Users.GetUsersPaged;

public sealed record GetUsersPagedQuery(
    int Page,
    int PageSize,
    QueryFilter? Filter = null) : IRequest<Result<PagedResponse<UserListItemDto>>>;