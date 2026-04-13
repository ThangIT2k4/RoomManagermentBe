using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record DepositRefundPaidEvent(
    Guid RefundId,
    Guid OrganizationId,
    Guid LeaseId,
    Guid? TenantId,
    decimal Amount,
    DateTime PaidAtUtc) : DomainEvent();
