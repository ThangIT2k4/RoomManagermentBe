using Identity.Application.Common;
using Identity.Domain.Common;
using MediatR;

namespace Identity.Application.Features.Permissions.GetPermissionsPaged;

public sealed record GetPermissionsPagedQuery(
    int Page,
    int PageSize,
    QueryFilter? Filter = null) : IRequest<Result<PagedResponse<PermissionListItemDto>>>;
