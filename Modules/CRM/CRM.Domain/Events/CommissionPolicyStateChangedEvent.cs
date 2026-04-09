using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record CommissionPolicyStateChangedEvent(
    Guid PolicyId,
    Guid OrganizationId,
    bool IsActive,
    DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
