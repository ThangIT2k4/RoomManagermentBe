using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.GetResidents;

public sealed class GetResidentsQueryHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<GetResidentsQuery, IReadOnlyList<LeaseResidentDto>>
{
    public Task<IReadOnlyList<LeaseResidentDto>> Handle(GetResidentsQuery request, CancellationToken cancellationToken)
        => leaseService.GetResidentsAsync(request.OrganizationId, request.LeaseId, cancellationToken);
}
