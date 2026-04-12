using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.MasterLeases.TerminateMasterLease;

public sealed class TerminateMasterLeaseCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<TerminateMasterLeaseCommand, MasterLeaseDto?>
{
    public Task<MasterLeaseDto?> Handle(TerminateMasterLeaseCommand request, CancellationToken cancellationToken)
        => leaseService.TerminateMasterLeaseAsync(request.OrganizationId, request.UserId, request.Id, request.TerminationDate, request.Reason, cancellationToken);
}
