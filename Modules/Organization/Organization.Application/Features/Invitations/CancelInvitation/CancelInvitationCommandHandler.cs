using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.CancelInvitation;

public sealed class CancelInvitationCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<CancelInvitationCommand, Result>
{
    public Task<Result> Handle(CancelInvitationCommand request, CancellationToken cancellationToken)
        => service.CancelInvitationAsync(
            new CancelInvitationRequest(request.OrganizationId, request.MembershipId, request.ActorUserId),
            cancellationToken);
}
