using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Roles.GetRolesPaged;

public sealed class GetRolesPagedQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRolesPagedQuery, Result<PagedResponse<RoleListItemDto>>>
{
    public async Task<Result<PagedResponse<RoleListItemDto>>> Handle(
        GetRolesPagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var paged = await roleRepository.GetPagedAsync(query.Page, query.PageSize, query.Filter, cancellationToken);

        var items = paged.Items
            .Select(role => new RoleListItemDto(
                role.Id,
                role.Code.Value,
                role.Name.Value))
            .ToList();

        var response = new PagedResponse<RoleListItemDto>(
            items,
            paged.TotalCount,
            paged.Page,
            paged.PageSize,
            paged.TotalPages,
            paged.HasPreviousPage,
            paged.HasNextPage);

        return Result<PagedResponse<RoleListItemDto>>.Success(response);
    }
}
