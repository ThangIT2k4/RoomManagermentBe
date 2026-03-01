using Identity.Application.Common;
using Identity.Domain.Common;
using MediatR;

namespace Identity.Application.Features.Roles.GetRolesPaged;

public sealed record GetRolesPagedQuery(
    int Page,
    int PageSize,
    QueryFilter? Filter = null) : IRequest<Result<PagedResponse<RoleListItemDto>>>;
