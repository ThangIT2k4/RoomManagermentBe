using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.LinkResidentUser;

public sealed class LinkResidentUserCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<LinkResidentUserCommand, LeaseResidentDto?>
{
    public Task<LeaseResidentDto?> Handle(LinkResidentUserCommand request, CancellationToken cancellationToken)
        => leaseService.LinkResidentUserAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
