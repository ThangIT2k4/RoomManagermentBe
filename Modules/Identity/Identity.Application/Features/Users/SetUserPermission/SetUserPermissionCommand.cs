using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Users.SetUserPermission;

public sealed record SetUserPermissionCommand(
    Guid UserId,
    Guid PermissionId,
    bool IsGranted) : IRequest<Result>;

