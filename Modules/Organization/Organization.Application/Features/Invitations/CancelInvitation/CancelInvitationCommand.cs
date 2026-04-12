using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.CancelInvitation;

public sealed record CancelInvitationCommand(Guid OrganizationId, Guid MembershipId, Guid ActorUserId)
    : IAppRequest<Result>;
