namespace Identity.Application.Features.Menus.GetMenusPaged;

public sealed record MenuPagedListItemDto(
    Guid Id,
    string Code,
    string Label,
    int OrderIndex,
    Guid? ParentId,
    bool IsActive);
