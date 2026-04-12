using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.SetPrimaryResident;

public sealed class SetPrimaryResidentCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<SetPrimaryResidentCommand, bool>
{
    public Task<bool> Handle(SetPrimaryResidentCommand request, CancellationToken cancellationToken)
        => leaseService.SetPrimaryResidentAsync(request.OrganizationId, request.UserId, request.LeaseId, request.ResidentId, cancellationToken);
}
