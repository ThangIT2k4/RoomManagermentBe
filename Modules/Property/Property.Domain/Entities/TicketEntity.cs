using Property.Domain.Common;
using Property.Domain.Events;
using Property.Domain.Exceptions;
using Property.Domain.ValueObjects;

namespace Property.Domain.Entities;

public sealed class TicketEntity : AggregateRoot<Guid>
{
    public Guid OrganizationId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid? UnitId { get; private set; }
    public Guid? LeaseId { get; private set; }
    public Guid CreatedBy { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid? PriorityId { get; private set; }
    public string Status { get; private set; }
    public Guid? AssignedTo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private TicketEntity(
        Guid id,
        Guid organizationId,
        Guid propertyId,
        Guid? unitId,
        Guid? leaseId,
        Guid createdBy,
        string title,
        string description,
        Guid? priorityId,
        string status,
        Guid? assignedTo,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        PropertyId = propertyId;
        UnitId = unitId;
        LeaseId = leaseId;
        CreatedBy = createdBy;
        Title = title;
        Description = description;
        PriorityId = priorityId;
        Status = status;
        AssignedTo = assignedTo;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static TicketEntity FromPersistence(
        Guid id,
        Guid organizationId,
        Guid propertyId,
        Guid? unitId,
        Guid? leaseId,
        Guid createdBy,
        string title,
        string description,
        Guid? priorityId,
        string status,
        Guid? assignedTo,
        DateTime createdAt,
        DateTime? updatedAt)
        => new(id, organizationId, propertyId, unitId, leaseId, createdBy, TicketTitle.Create(title).Value, description, priorityId, status, assignedTo, createdAt, updatedAt);

    public void ChangeStatus(string newStatus)
    {
        var old = Status;
        var normalized = newStatus.Trim().ToLowerInvariant();
        var valid = (old, normalized) switch
        {
            ("open", "in_progress") => true,
            ("in_progress", "resolved") => true,
            ("resolved", "closed") => true,
            ("open", "cancelled") => true,
            ("in_progress", "cancelled") => true,
            (_, _) when old == normalized => true,
            _ => false
        };

        if (!valid)
        {
            throw new InvalidTicketStatusTransitionException($"Invalid ticket status transition: {old} -> {normalized}");
        }

        Status = normalized;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new TicketStatusChangedEvent(Id, old, normalized, DateTimeOffset.UtcNow));
    }
}
