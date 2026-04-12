using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.ServiceSets.UpsertServiceSet;

public sealed class UpsertServiceSetCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<UpsertServiceSetCommand, LeaseServiceSetDto>
{
    public Task<LeaseServiceSetDto> Handle(UpsertServiceSetCommand request, CancellationToken cancellationToken)
        => leaseService.UpsertServiceSetAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
