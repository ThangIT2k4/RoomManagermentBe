namespace Property.Domain.Entities;

public sealed class TicketEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public Guid PropertyId { get; }
    public Guid? UnitId { get; }
    public Guid? LeaseId { get; }
    public Guid CreatedBy { get; }
    public string Title { get; }
    public string Description { get; }
    public Guid? PriorityId { get; }
    public string Status { get; }
    public Guid? AssignedTo { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

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
        => new(id, organizationId, propertyId, unitId, leaseId, createdBy, title, description, priorityId, status, assignedTo, createdAt, updatedAt);
}
