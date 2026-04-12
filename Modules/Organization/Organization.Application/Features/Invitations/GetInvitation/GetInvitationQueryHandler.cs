using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.GetInvitation;

public sealed class GetInvitationQueryHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<GetInvitationQuery, Result<InvitationPreviewDto>>
{
    public Task<Result<InvitationPreviewDto>> Handle(GetInvitationQuery request, CancellationToken cancellationToken)
        => service.GetInvitationAsync(request.Token, cancellationToken);
}
