using CRM.Domain.Common;
using CRM.Domain.Enums;
using CRM.Domain.Exceptions;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Entities;

public sealed class CommissionEventEntity
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? PolicyId { get; private set; }
    public Guid? AgentId { get; private set; }
    public decimal CommissionTotal { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public string Status { get; private set; }
    public string? TriggerEvent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CommissionEventEntity(
        Guid id,
        Guid organizationId,
        Guid? policyId,
        Guid? agentId,
        decimal commissionTotal,
        DateTime occurredAt,
        string status,
        string? triggerEvent,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        PolicyId = policyId;
        AgentId = agentId;
        CommissionTotal = commissionTotal;
        OccurredAt = occurredAt;
        Status = status;
        TriggerEvent = triggerEvent;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static CommissionEventEntity Create(
        Guid organizationId,
        decimal commissionTotal,
        DateTime occurredAt,
        string status,
        Guid? policyId = null,
        Guid? agentId = null,
        string? triggerEvent = null,
        DateTime? createdAt = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new DomainValidationException("OrganizationId is required.");
        }

        return new CommissionEventEntity(
            Guid.NewGuid(),
            organizationId,
            policyId,
            agentId,
            MoneyAmount.Create(commissionTotal, allowZero: true, nameof(commissionTotal)).Value,
            occurredAt,
            EnumValueParser.ParseRequired<CommissionEventStatus>(status, nameof(status)),
            EnumValueParser.ParseOptional<CommissionTriggerEventType>(triggerEvent, nameof(triggerEvent)),
            createdAt ?? DateTime.UtcNow,
            null);
    }

    public static CommissionEventEntity Reconstitute(
        Guid id,
        Guid organizationId,
        Guid? policyId,
        Guid? agentId,
        decimal commissionTotal,
        DateTime occurredAt,
        string status,
        string? triggerEvent,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new CommissionEventEntity(id, organizationId, policyId, agentId, commissionTotal, occurredAt, status, triggerEvent, createdAt, updatedAt);
    }
}
