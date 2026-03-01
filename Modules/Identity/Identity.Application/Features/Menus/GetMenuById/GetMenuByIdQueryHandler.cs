using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Menus.GetMenuById;

public sealed class GetMenuByIdQueryHandler(IMenuRepository menuRepository)
    : IRequestHandler<GetMenuByIdQuery, Result<MenuDto>>
{
    public async Task<Result<MenuDto>> Handle(GetMenuByIdQuery query, CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(query.MenuId, cancellationToken);

        if (menu is null)
        {
            return Result<MenuDto>.Failure(
                new Error("Menu.NotFound", $"Menu with id '{query.MenuId}' was not found."));
        }

        var dto = new MenuDto(
            menu.Id,
            menu.Code.Value,
            menu.Label.Value,
            menu.OrderIndex,
            menu.ParentId,
            menu.IsActive);

        return Result<MenuDto>.Success(dto);
    }
}

