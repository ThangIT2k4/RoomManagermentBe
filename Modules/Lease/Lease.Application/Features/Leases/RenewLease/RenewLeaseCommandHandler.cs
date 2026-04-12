using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.RenewLease;

public sealed class RenewLeaseCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<RenewLeaseCommand, LeaseDto>
{
    public Task<LeaseDto> Handle(RenewLeaseCommand request, CancellationToken cancellationToken)
        => leaseService.RenewLeaseAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
