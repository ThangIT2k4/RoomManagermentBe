using Identity.Domain.Common;

namespace Identity.Application.Features.Menus.GetMenusByParent;

public sealed record GetMenusByParentQuery(
    Guid? ParentId,
    QueryFilter? Filter = null);

