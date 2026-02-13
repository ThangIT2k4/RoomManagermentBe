using Identity.Application.Common;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Menus.GetMenusByParent;

public sealed class GetMenusByParentQueryHandler(IMenuRepository menuRepository)
{
    public async Task<Result<IReadOnlyList<MenuListItemDto>>> HandleAsync(
        GetMenusByParentQuery query,
        CancellationToken cancellationToken = default)
    {
        var menus = await menuRepository.GetByParentIdAsync(query.ParentId, cancellationToken);

        var dtos = menus
            .Select(menu => new MenuListItemDto(
                menu.Id,
                menu.Code.Value,
                menu.Label.Value,
                menu.OrderIndex,
                menu.ParentId,
                menu.IsActive))
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<MenuListItemDto>>.Success(dtos);
    }
}

