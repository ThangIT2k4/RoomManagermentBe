using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.UpdateLease;

public sealed class UpdateLeaseCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<UpdateLeaseCommand, LeaseDto?>
{
    public Task<LeaseDto?> Handle(UpdateLeaseCommand request, CancellationToken cancellationToken)
        => leaseService.UpdateLeaseAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
