namespace Identity.Application.Features.Permissions.GetPermissionsPaged;

public sealed record PermissionListItemDto(
    Guid Id,
    string Code,
    string Name);
