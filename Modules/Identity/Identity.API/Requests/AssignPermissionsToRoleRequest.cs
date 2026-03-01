namespace Identity.API.Requests;

public sealed record AssignPermissionsToRoleRequest(IReadOnlyCollection<Guid> PermissionIds);

