namespace Identity.Application.Features.Users.SetUserPermission;

public sealed record SetUserPermissionCommand(
    Guid UserId,
    Guid PermissionId,
    bool IsGranted);

