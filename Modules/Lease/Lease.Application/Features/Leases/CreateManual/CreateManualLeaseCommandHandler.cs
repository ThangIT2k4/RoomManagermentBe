using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.CreateManual;

public sealed class CreateManualLeaseCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<CreateManualLeaseCommand, LeaseDto>
{
    public Task<LeaseDto> Handle(CreateManualLeaseCommand request, CancellationToken cancellationToken)
        => leaseService.CreateManualAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
