using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.PaymentCycles.DeletePaymentCycle;

public sealed record DeletePaymentCycleCommand(Guid OrganizationId, Guid UserId, Guid Id) : IAppRequest<bool>;
