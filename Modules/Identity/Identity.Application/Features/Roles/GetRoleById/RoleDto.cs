namespace Identity.Application.Features.Roles.GetRoleById;

public sealed record RoleDto(
    Guid Id,
    string Code,
    string Name);

