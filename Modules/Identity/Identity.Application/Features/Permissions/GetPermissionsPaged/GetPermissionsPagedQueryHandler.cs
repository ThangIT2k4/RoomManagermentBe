using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Permissions.GetPermissionsPaged;

public sealed class GetPermissionsPagedQueryHandler(IPermissionRepository permissionRepository)
    : IRequestHandler<GetPermissionsPagedQuery, Result<PagedResponse<PermissionListItemDto>>>
{
    public async Task<Result<PagedResponse<PermissionListItemDto>>> Handle(
        GetPermissionsPagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var paged = await permissionRepository.GetPagedAsync(query.Page, query.PageSize, query.Filter, cancellationToken);

        var items = paged.Items
            .Select(permission => new PermissionListItemDto(
                permission.Id,
                permission.Code.Value,
                permission.Name))
            .ToList();

        var response = new PagedResponse<PermissionListItemDto>(
            items,
            paged.TotalCount,
            paged.Page,
            paged.PageSize,
            paged.TotalPages,
            paged.HasPreviousPage,
            paged.HasNextPage);

        return Result<PagedResponse<PermissionListItemDto>>.Success(response);
    }
}
