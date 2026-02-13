namespace Identity.Application.Features.Roles.AssignPermissionsToRole;

public sealed record AssignPermissionsToRoleCommand(
    Guid RoleId,
    IReadOnlyCollection<Guid> PermissionIds);

