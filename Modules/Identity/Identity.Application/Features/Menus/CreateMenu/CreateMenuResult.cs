namespace Identity.Application.Features.Menus.CreateMenu;

public sealed record CreateMenuResult(
    Guid Id,
    string Code,
    string Label,
    int OrderIndex,
    Guid? ParentId,
    bool IsActive
);

