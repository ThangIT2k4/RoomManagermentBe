using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Menus.GetMenusByParent;

public sealed class GetMenusByParentQueryHandler(IMenuRepository menuRepository)
    : IRequestHandler<GetMenusByParentQuery, Result<IReadOnlyList<MenuListItemDto>>>
{
    public async Task<Result<IReadOnlyList<MenuListItemDto>>> Handle(
        GetMenusByParentQuery query,
        CancellationToken cancellationToken = default)
    {
        var menus = await menuRepository.GetByParentIdAsync(query.ParentId, query.Filter, cancellationToken);

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

