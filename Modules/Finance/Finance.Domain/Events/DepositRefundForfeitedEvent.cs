using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record DepositRefundForfeitedEvent(
    Guid RefundId,
    Guid OrganizationId,
    Guid LeaseId,
    Guid? TenantId,
    string Reason) : DomainEvent();
