using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.CanMember;

public sealed class CanMemberQueryHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<CanMemberQuery, Result<bool>>
{
    public Task<Result<bool>> Handle(CanMemberQuery request, CancellationToken cancellationToken)
        => service.CanMemberAsync(
            new CanMemberRequest(request.OrganizationId, request.UserId, request.CapabilityKey),
            cancellationToken);
}
