using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.PaymentCycles.DeletePaymentCycle;

public sealed class DeletePaymentCycleCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<DeletePaymentCycleCommand, bool>
{
    public Task<bool> Handle(DeletePaymentCycleCommand request, CancellationToken cancellationToken)
        => leaseService.DeletePaymentCycleAsync(request.OrganizationId, request.UserId, request.Id, cancellationToken);
}
