using Identity.Application.Common;
using Identity.Domain.Common;
using MediatR;

namespace Identity.Application.Features.Menus.GetMenusByParent;

public sealed record GetMenusByParentQuery(
    Guid? ParentId,
    QueryFilter? Filter = null) : IRequest<Result<IReadOnlyList<MenuListItemDto>>>;

