using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.AddResident;

public sealed class AddResidentCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<AddResidentCommand, LeaseResidentDto>
{
    public Task<LeaseResidentDto> Handle(AddResidentCommand request, CancellationToken cancellationToken)
        => leaseService.AddResidentAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
