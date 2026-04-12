using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.ServiceSets.ApplyServiceSet;

public sealed class ApplyServiceSetCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<ApplyServiceSetCommand, bool>
{
    public Task<bool> Handle(ApplyServiceSetCommand request, CancellationToken cancellationToken)
        => leaseService.ApplyServiceSetAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
