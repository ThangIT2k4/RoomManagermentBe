using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.UpsertMember;

public sealed class UpsertMemberCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<UpsertMemberCommand, Result<OrganizationMemberDto>>
{
    public Task<Result<OrganizationMemberDto>> Handle(UpsertMemberCommand request, CancellationToken cancellationToken)
        => service.UpsertMemberAsync(
            new UpsertMemberRequest(request.OrganizationId, request.UserId, request.RoleId, request.ActorUserId),
            cancellationToken);
}
