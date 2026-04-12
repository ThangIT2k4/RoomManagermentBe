using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.ServiceSets.GetServiceSetById;

public sealed class GetServiceSetByIdQueryHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<GetServiceSetByIdQuery, LeaseServiceSetDto?>
{
    public Task<LeaseServiceSetDto?> Handle(GetServiceSetByIdQuery request, CancellationToken cancellationToken)
        => leaseService.GetServiceSetByIdAsync(request.OrganizationId, request.ServiceSetId, cancellationToken);
}
