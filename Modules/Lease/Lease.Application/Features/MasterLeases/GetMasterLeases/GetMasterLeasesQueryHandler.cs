using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.MasterLeases.GetMasterLeases;

public sealed class GetMasterLeasesQueryHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<GetMasterLeasesQuery, IReadOnlyList<MasterLeaseDto>>
{
    public Task<IReadOnlyList<MasterLeaseDto>> Handle(GetMasterLeasesQuery request, CancellationToken cancellationToken)
        => leaseService.GetMasterLeasesAsync(request.OrganizationId, cancellationToken);
}
