namespace Identity.Application.Features.Permissions.CreatePermission;

public sealed record CreatePermissionCommand(
    string Code,
    string Name);

