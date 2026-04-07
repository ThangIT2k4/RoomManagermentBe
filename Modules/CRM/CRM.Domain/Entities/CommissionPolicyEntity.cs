using CRM.Domain.Common;
using CRM.Domain.Enums;
using CRM.Domain.Exceptions;
using CRM.Domain.Events;

namespace CRM.Domain.Entities;

public sealed class CommissionPolicyEntity : AggregateRoot<Guid>
{
    public Guid OrganizationId { get; private set; }
    public string Title { get; private set; }
    public string CalcType { get; private set; }
    public string TriggerEvent { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CommissionPolicyEntity(
        Guid id,
        Guid organizationId,
        string title,
        string calcType,
        string triggerEvent,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        Title = title;
        CalcType = calcType;
        TriggerEvent = triggerEvent;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static CommissionPolicyEntity Create(
        Guid organizationId,
        string title,
        string calcType,
        string triggerEvent,
        bool isActive = true,
        DateTime? createdAt = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new DomainValidationException("OrganizationId is required.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainValidationException("Title is required.");
        }

        return new CommissionPolicyEntity(
            Guid.NewGuid(),
            organizationId,
            title.Trim(),
            EnumValueParser.ParseRequired<CommissionCalculationType>(calcType, nameof(calcType)),
            EnumValueParser.ParseRequired<CommissionTriggerEventType>(triggerEvent, nameof(triggerEvent)),
            isActive,
            createdAt ?? DateTime.UtcNow,
            null);
    }

    public static CommissionPolicyEntity Reconstitute(
        Guid id,
        Guid organizationId,
        string title,
        string calcType,
        string triggerEvent,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new CommissionPolicyEntity(id, organizationId, title, calcType, triggerEvent, isActive, createdAt, updatedAt);
    }

    public void Activate(DateTime? updatedAt = null)
    {
        IsActive = true;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new CommissionPolicyStateChangedEvent(Id, OrganizationId, IsActive, DateTimeOffset.UtcNow));
    }

    public void Deactivate(DateTime? updatedAt = null)
    {
        IsActive = false;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new CommissionPolicyStateChangedEvent(Id, OrganizationId, IsActive, DateTimeOffset.UtcNow));
    }
}
