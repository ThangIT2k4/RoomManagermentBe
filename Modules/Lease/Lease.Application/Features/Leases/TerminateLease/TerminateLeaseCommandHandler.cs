using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.TerminateLease;

public sealed class TerminateLeaseCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<TerminateLeaseCommand, LeaseDto?>
{
    public Task<LeaseDto?> Handle(TerminateLeaseCommand request, CancellationToken cancellationToken)
        => leaseService.TerminateLeaseAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
