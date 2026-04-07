namespace Lease.Domain.Entities;

public sealed class LeaseServiceSetItemEntity
{
    public Guid Id { get; }
    public Guid ServiceId { get; }
    public decimal Price { get; }
    public bool IsRequired { get; }

    private LeaseServiceSetItemEntity(Guid id, Guid serviceId, decimal price, bool isRequired)
    {
        Id = id;
        ServiceId = serviceId;
        Price = price;
        IsRequired = isRequired;
    }

    public static LeaseServiceSetItemEntity Create(Guid serviceId, decimal price, bool isRequired, Guid? id = null)
    {
        if (serviceId == Guid.Empty) throw new ArgumentException("ServiceId is required.", nameof(serviceId));
        if (price < 0) throw new ArgumentException("Price must be >= 0.", nameof(price));
        return new LeaseServiceSetItemEntity(id ?? Guid.NewGuid(), serviceId, price, isRequired);
    }
}
