using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record DepositRefundCreatedEvent(
    Guid RefundId,
    Guid OrganizationId,
    Guid LeaseId,
    Guid? TenantId,
    decimal Amount) : DomainEvent();
