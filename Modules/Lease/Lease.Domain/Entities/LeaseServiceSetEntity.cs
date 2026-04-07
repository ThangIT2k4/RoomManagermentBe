namespace Lease.Domain.Entities;

public sealed class LeaseServiceSetEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public string Name { get; }
    public string? Description { get; }
    public IReadOnlyList<LeaseServiceSetItemEntity> Items { get; }

    private LeaseServiceSetEntity(Guid id, Guid organizationId, string name, string? description, IReadOnlyList<LeaseServiceSetItemEntity> items)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        Description = description;
        Items = items;
    }

    public static LeaseServiceSetEntity Create(Guid organizationId, string name, string? description, IReadOnlyList<LeaseServiceSetItemEntity> items)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("OrganizationId is required.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (items.Count == 0) throw new ArgumentException("At least one service item is required.", nameof(items));
        if (items.GroupBy(x => x.ServiceId).Any(g => g.Count() > 1)) throw new InvalidOperationException("Duplicate service_id in same set is not allowed.");
        return new LeaseServiceSetEntity(Guid.NewGuid(), organizationId, name.Trim(), description?.Trim(), items);
    }
}
