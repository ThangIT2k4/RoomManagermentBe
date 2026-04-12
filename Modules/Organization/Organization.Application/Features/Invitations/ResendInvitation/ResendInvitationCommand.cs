using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.ResendInvitation;

public sealed record ResendInvitationCommand(Guid OrganizationId, Guid MembershipId, Guid ActorUserId)
    : IAppRequest<Result<InvitationPreviewDto>>;
