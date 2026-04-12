using Lease.Application.Dtos;
using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.PaymentCycles.GetPaymentCycles;

public sealed class GetPaymentCyclesQueryHandler(ILeaseApplicationService leaseService)
    : IAppRequestHandler<GetPaymentCyclesQuery, IReadOnlyList<PaymentCycleDto>>
{
    public Task<IReadOnlyList<PaymentCycleDto>> Handle(GetPaymentCyclesQuery request, CancellationToken cancellationToken)
        => leaseService.GetPaymentCyclesAsync(request.OrganizationId, cancellationToken);
}
