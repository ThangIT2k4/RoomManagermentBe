namespace Property.Domain.Entities;

public sealed class UnitEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public Guid PropertyId { get; }
    public string Code { get; }
    public string? Name { get; }
    public short Status { get; }
    public decimal BaseRent { get; }
    public decimal? DepositAmount { get; }
    public int? Floor { get; }
    public decimal? AreaM2 { get; }
    public string? UnitType { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private UnitEntity(
        Guid id,
        Guid organizationId,
        Guid propertyId,
        string code,
        string? name,
        short status,
        decimal baseRent,
        decimal? depositAmount,
        int? floor,
        decimal? areaM2,
        string? unitType,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        PropertyId = propertyId;
        Code = code;
        Name = name;
        Status = status;
        BaseRent = baseRent;
        DepositAmount = depositAmount;
        Floor = floor;
        AreaM2 = areaM2;
        UnitType = unitType;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UnitEntity FromPersistence(
        Guid id,
        Guid organizationId,
        Guid propertyId,
        string code,
        string? name,
        short status,
        decimal baseRent,
        decimal? depositAmount,
        int? floor,
        decimal? areaM2,
        string? unitType,
        DateTime createdAt,
        DateTime? updatedAt)
        => new(id, organizationId, propertyId, code, name, status, baseRent, depositAmount, floor, areaM2, unitType, createdAt, updatedAt);
}
