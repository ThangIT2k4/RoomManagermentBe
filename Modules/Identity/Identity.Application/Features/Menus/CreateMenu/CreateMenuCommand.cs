namespace Identity.Application.Features.Menus.CreateMenu;

public sealed record CreateMenuCommand(
    string Code,
    string Label,
    int OrderIndex,
    string? Path,
    string? Icon,
    Guid? ParentId,
    bool IsActive
);

