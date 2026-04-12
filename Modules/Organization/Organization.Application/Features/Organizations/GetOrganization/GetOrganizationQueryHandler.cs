using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Organizations.GetOrganization;

public sealed class GetOrganizationQueryHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<GetOrganizationQuery, Result<OrganizationDto>>
{
    public Task<Result<OrganizationDto>> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
        => service.GetOrganizationAsync(request.OrganizationId, cancellationToken);
}
