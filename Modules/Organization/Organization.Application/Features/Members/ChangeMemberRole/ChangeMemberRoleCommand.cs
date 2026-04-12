using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.ChangeMemberRole;

public sealed record ChangeMemberRoleCommand(Guid OrganizationId, Guid UserId, Guid RoleId, Guid ActorUserId)
    : IAppRequest<Result>;
