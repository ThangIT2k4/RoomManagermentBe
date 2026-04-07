namespace Property.Domain.Entities;

public sealed class MeterEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public Guid PropertyId { get; }
    public Guid? UnitId { get; }
    public string MeterType { get; }
    public string MeterNumber { get; }
    public bool IsActive { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private MeterEntity(
        Guid id,
        Guid organizationId,
        Guid propertyId,
        Guid? unitId,
        string meterType,
        string meterNumber,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        PropertyId = propertyId;
        UnitId = unitId;
        MeterType = meterType;
        MeterNumber = meterNumber;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static MeterEntity FromPersistence(
        Guid id,
        Guid organizationId,
        Guid propertyId,
        Guid? unitId,
        string meterType,
        string meterNumber,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt)
        => new(id, organizationId, propertyId, unitId, meterType, meterNumber, isActive, createdAt, updatedAt);
}
