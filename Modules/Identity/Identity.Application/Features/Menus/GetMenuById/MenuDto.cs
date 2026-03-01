namespace Identity.Application.Features.Menus.GetMenuById;

public sealed record MenuDto(
    Guid Id,
    string Code,
    string Label,
    int OrderIndex,
    Guid? ParentId,
    bool IsActive);

