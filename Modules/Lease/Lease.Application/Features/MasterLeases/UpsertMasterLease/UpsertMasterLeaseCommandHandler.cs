using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.MasterLeases.UpsertMasterLease;

public sealed class UpsertMasterLeaseCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<UpsertMasterLeaseCommand, MasterLeaseDto>
{
    public Task<MasterLeaseDto> Handle(UpsertMasterLeaseCommand request, CancellationToken cancellationToken)
        => leaseService.UpsertMasterLeaseAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
