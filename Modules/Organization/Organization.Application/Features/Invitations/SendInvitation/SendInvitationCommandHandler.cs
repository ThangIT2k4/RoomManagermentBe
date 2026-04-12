using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.SendInvitation;

public sealed class SendInvitationCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<SendInvitationCommand, Result<InvitationPreviewDto>>
{
    public Task<Result<InvitationPreviewDto>> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
        => service.SendInvitationAsync(
            new SendInvitationRequest(request.OrganizationId, request.Email, request.RoleId, request.ActorUserId, request.Note),
            cancellationToken);
}
