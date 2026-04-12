using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.GetTenantLeases;

public sealed class GetTenantLeasesQueryHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<GetTenantLeasesQuery, IReadOnlyList<LeaseDto>>
{
    public Task<IReadOnlyList<LeaseDto>> Handle(GetTenantLeasesQuery request, CancellationToken cancellationToken)
        => leaseService.GetTenantLeasesAsync(request.UserId, cancellationToken);
}
