using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.GetLeaseById;

public sealed class GetLeaseByIdQueryHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<GetLeaseByIdQuery, LeaseDto?>
{
    public Task<LeaseDto?> Handle(GetLeaseByIdQuery request, CancellationToken cancellationToken)
        => leaseService.GetLeaseByIdAsync(request.OrganizationId, request.LeaseId, cancellationToken);
}
