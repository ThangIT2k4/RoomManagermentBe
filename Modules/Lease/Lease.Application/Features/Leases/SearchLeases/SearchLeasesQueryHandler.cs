using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.SearchLeases;

public sealed class SearchLeasesQueryHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<SearchLeasesQuery, IReadOnlyList<LeaseDto>>
{
    public Task<IReadOnlyList<LeaseDto>> Handle(SearchLeasesQuery request, CancellationToken cancellationToken)
        => leaseService.SearchLeasesAsync(request.OrganizationId, request.Statuses, request.UnitId, request.Search, request.Page, request.PerPage, cancellationToken);
}
