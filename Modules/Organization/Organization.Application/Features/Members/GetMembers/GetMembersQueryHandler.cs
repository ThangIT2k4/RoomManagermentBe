using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.GetMembers;

public sealed class GetMembersQueryHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<GetMembersQuery, Result<PagedResponse<OrganizationMemberDto>>>
{
    public Task<Result<PagedResponse<OrganizationMemberDto>>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        => service.GetMembersAsync(request.OrganizationId, request.Page, request.PageSize, cancellationToken);
}
