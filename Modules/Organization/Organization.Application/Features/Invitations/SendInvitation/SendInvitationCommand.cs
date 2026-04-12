using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.SendInvitation;

public sealed record SendInvitationCommand(Guid OrganizationId, string Email, Guid RoleId, Guid ActorUserId, string? Note)
    : IAppRequest<Result<InvitationPreviewDto>>;
