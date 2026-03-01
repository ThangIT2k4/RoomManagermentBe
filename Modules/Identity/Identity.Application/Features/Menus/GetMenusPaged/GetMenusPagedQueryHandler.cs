using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Menus.GetMenusPaged;

public sealed class GetMenusPagedQueryHandler(IMenuRepository menuRepository)
    : IRequestHandler<GetMenusPagedQuery, Result<PagedResponse<MenuPagedListItemDto>>>
{
    public async Task<Result<PagedResponse<MenuPagedListItemDto>>> Handle(
        GetMenusPagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var paged = await menuRepository.GetPagedAsync(query.Page, query.PageSize, query.Filter, cancellationToken);

        var items = paged.Items
            .Select(menu => new MenuPagedListItemDto(
                menu.Id,
                menu.Code.Value,
                menu.Label.Value,
                menu.OrderIndex,
                menu.ParentId,
                menu.IsActive))
            .ToList();

        var response = new PagedResponse<MenuPagedListItemDto>(
            items,
            paged.TotalCount,
            paged.Page,
            paged.PageSize,
            paged.TotalPages,
            paged.HasPreviousPage,
            paged.HasNextPage);

        return Result<PagedResponse<MenuPagedListItemDto>>.Success(response);
    }
}
