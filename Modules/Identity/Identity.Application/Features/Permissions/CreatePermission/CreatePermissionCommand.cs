using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Permissions.CreatePermission;

public sealed record CreatePermissionCommand(
    string Code,
    string Name) : IRequest<Result<CreatePermissionResult>>;

