namespace Identity.Application.Features.Roles.GetRolesPaged;

public sealed record RoleListItemDto(
    Guid Id,
    string Code,
    string Name);
