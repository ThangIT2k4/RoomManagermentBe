using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.RemoveMember;

public sealed record RemoveMemberCommand(Guid OrganizationId, Guid UserId, Guid ActorUserId, string? Reason)
    : IAppRequest<Result>;
