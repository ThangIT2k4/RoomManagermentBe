using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.PaymentCycles.GetPaymentCycles;

public sealed record GetPaymentCyclesQuery(Guid OrganizationId) : IAppRequest<IReadOnlyList<PaymentCycleDto>>;
