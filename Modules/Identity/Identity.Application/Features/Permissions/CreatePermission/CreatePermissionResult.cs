namespace Identity.Application.Features.Permissions.CreatePermission;

public sealed record CreatePermissionResult(
    Guid Id,
    string Code,
    string Name);

