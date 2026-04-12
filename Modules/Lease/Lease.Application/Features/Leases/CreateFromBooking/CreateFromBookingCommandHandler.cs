using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.CreateFromBooking;

public sealed class CreateFromBookingCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<CreateFromBookingCommand, LeaseDto>
{
    public Task<LeaseDto> Handle(CreateFromBookingCommand request, CancellationToken cancellationToken)
        => leaseService.CreateFromBookingAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
