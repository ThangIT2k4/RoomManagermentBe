using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.AcceptInvitation;

public sealed class AcceptInvitationCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<AcceptInvitationCommand, Result>
{
    public Task<Result> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
        => service.AcceptInvitationAsync(
            new AcceptInvitationRequest(request.Token, request.UserId),
            cancellationToken);
}
