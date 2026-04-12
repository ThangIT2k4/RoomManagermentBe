using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.ResendInvitation;

public sealed class ResendInvitationCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<ResendInvitationCommand, Result<InvitationPreviewDto>>
{
    public Task<Result<InvitationPreviewDto>> Handle(ResendInvitationCommand request, CancellationToken cancellationToken)
        => service.ResendInvitationAsync(
            new ResendInvitationRequest(request.OrganizationId, request.MembershipId, request.ActorUserId),
            cancellationToken);
}
