using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.PaymentCycles.UpsertPaymentCycle;

public sealed class UpsertPaymentCycleCommandHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<UpsertPaymentCycleCommand, PaymentCycleDto>
{
    public Task<PaymentCycleDto> Handle(UpsertPaymentCycleCommand request, CancellationToken cancellationToken)
        => leaseService.UpsertPaymentCycleAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
