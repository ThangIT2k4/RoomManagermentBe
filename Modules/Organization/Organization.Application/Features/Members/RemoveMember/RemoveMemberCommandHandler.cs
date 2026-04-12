using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.RemoveMember;

public sealed class RemoveMemberCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<RemoveMemberCommand, Result>
{
    public Task<Result> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
        => service.RemoveMemberAsync(
            new RemoveMemberRequest(request.OrganizationId, request.UserId, request.ActorUserId, request.Reason),
            cancellationToken);
}
