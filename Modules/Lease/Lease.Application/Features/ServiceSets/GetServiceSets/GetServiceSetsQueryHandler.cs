using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.ServiceSets.GetServiceSets;

public sealed class GetServiceSetsQueryHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<GetServiceSetsQuery, IReadOnlyList<LeaseServiceSetDto>>
{
    public Task<IReadOnlyList<LeaseServiceSetDto>> Handle(GetServiceSetsQuery request, CancellationToken cancellationToken)
        => leaseService.GetServiceSetsAsync(request.OrganizationId, cancellationToken);
}
