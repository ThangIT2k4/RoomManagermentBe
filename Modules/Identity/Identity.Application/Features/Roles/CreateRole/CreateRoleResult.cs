namespace Identity.Application.Features.Roles.CreateRole;

public sealed record CreateRoleResult(
    Guid Id,
    string Code,
    string Name);

