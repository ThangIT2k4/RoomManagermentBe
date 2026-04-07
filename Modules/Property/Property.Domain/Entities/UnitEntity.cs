using Property.Domain.Common;
using Property.Domain.Events;
using Property.Domain.Exceptions;
using Property.Domain.ValueObjects;

namespace Property.Domain.Entities;

public sealed class UnitEntity : AggregateRoot<Guid>
{
    public Guid OrganizationId { get; private set; }
    public Guid PropertyId { get; private set; }
    public string Code { get; private set; }
    public string? Name { get; private set; }
    public short Status { get; private set; }
    public decimal BaseRent { get; private set; }
    public decimal? DepositAmount { get; private set; }
    public int? Floor { get; private set; }
    public decimal? AreaM2 { get; private set; }
    public string? UnitType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

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
        => new(id, organizationId, propertyId, UnitCode.Create(code).Value, name, status, baseRent, depositAmount, floor, areaM2, unitType, createdAt, updatedAt);

    public void ChangeStatus(short newStatus)
    {
        var old = Status;
        var valid = (old, newStatus) switch
        {
            (1, 3) => true,
            (2, 3) => true,
            (3, 1) => true,
            (_, _) when old == newStatus => true,
            _ => false
        };

        if (!valid)
        {
            throw new InvalidUnitStatusTransitionException($"Invalid unit status transition: {old} -> {newStatus}");
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UnitStatusChangedEvent(Id, old, newStatus, DateTimeOffset.UtcNow));
    }
}
