namespace Identity.Application.Features.Menus.GetMenusByParent;

public sealed record MenuListItemDto(
    Guid Id,
    string Code,
    string Label,
    int OrderIndex,
    Guid? ParentId,
    bool IsActive);

