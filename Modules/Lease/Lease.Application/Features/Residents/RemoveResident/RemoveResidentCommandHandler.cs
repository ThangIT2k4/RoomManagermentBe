using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.RemoveResident;

public sealed class RemoveResidentCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<RemoveResidentCommand, bool>
{
    public Task<bool> Handle(RemoveResidentCommand request, CancellationToken cancellationToken)
        => leaseService.RemoveResidentAsync(request.OrganizationId, request.UserId, request.LeaseId, request.ResidentId, cancellationToken);
}
