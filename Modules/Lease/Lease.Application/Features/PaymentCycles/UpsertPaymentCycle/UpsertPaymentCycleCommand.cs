using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.PaymentCycles.UpsertPaymentCycle;

public sealed record UpsertPaymentCycleCommand(
    Guid OrganizationId,
    Guid UserId,
    UpsertPaymentCycleRequest Request) : IAppRequest<PaymentCycleDto>;
