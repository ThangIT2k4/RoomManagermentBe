namespace Identity.Application.Features.Permissions.GetPermissionById;

public sealed record PermissionDto(
    Guid Id,
    string Code,
    string Name);

