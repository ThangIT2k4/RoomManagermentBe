using CRM.Domain.Common;
using CRM.Domain.Enums;
using CRM.Domain.Exceptions;
using CRM.Domain.Events;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Entities;

public sealed class LeadEntity : AggregateRoot<Guid>
{
    public Guid OrganizationId { get; }
    public string? FullName { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    private LeadEntity(Guid id, Guid organizationId, string? fullName, string status, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        FullName = fullName;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static LeadEntity Create(Guid organizationId, string? fullName, string status = "new", DateTime? createdAt = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new DomainValidationException("OrganizationId is required.");
        }

        var normalizedFullName = NormalizeFullName(fullName);
        var normalizedStatus = NormalizeStatus(status);
        var entity = new LeadEntity(Guid.NewGuid(), organizationId, normalizedFullName, normalizedStatus, createdAt ?? DateTime.UtcNow, null);
        entity.AddDomainEvent(new LeadCreatedEvent(entity.Id, entity.OrganizationId, entity.FullName, entity.Status, DateTimeOffset.UtcNow));
        return entity;
    }

    public static LeadEntity FromPersistence(Guid id, Guid organizationId, string? fullName, string status, DateTime createdAt, DateTime? updatedAt)
    {
        return new LeadEntity(id, organizationId, fullName, status, createdAt, updatedAt);
    }

    public void UpdateStatus(string status, DateTime? updatedAt = null)
    {
        var normalizedStatus = NormalizeStatus(status);
        if (string.Equals(Status, normalizedStatus, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var previousStatus = Status;
        Status = normalizedStatus;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new LeadStatusChangedEvent(Id, previousStatus, Status, DateTimeOffset.UtcNow));
    }

    private static string? NormalizeFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return null;
        }

        return PersonName.Create(fullName, nameof(fullName)).Value;
    }

    private static string NormalizeStatus(string status)
    {
        return EnumValueParser.ParseRequired<LeadStatus>(status, nameof(status));
    }
}

