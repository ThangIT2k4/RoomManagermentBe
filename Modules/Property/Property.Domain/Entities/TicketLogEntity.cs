namespace Property.Domain.Entities;

public sealed class TicketLogEntity
{
    public Guid Id { get; }
    public Guid TicketId { get; }
    public Guid ActorId { get; }
    public string Action { get; }
    public string? Detail { get; }
    public decimal? CostAmount { get; }
    public string? CostNote { get; }
    public Guid? VendorId { get; }
    public string? ChargeTo { get; }
    public int? WarrantyPeriodDays { get; }
    public DateTime? WarrantyExpiresAt { get; }
    public DateTime CreatedAt { get; }

    private TicketLogEntity(
        Guid id,
        Guid ticketId,
        Guid actorId,
        string action,
        string? detail,
        decimal? costAmount,
        string? costNote,
        Guid? vendorId,
        string? chargeTo,
        int? warrantyPeriodDays,
        DateTime? warrantyExpiresAt,
        DateTime createdAt)
    {
        Id = id;
        TicketId = ticketId;
        ActorId = actorId;
        Action = action;
        Detail = detail;
        CostAmount = costAmount;
        CostNote = costNote;
        VendorId = vendorId;
        ChargeTo = chargeTo;
        WarrantyPeriodDays = warrantyPeriodDays;
        WarrantyExpiresAt = warrantyExpiresAt;
        CreatedAt = createdAt;
    }

    public static TicketLogEntity FromPersistence(
        Guid id,
        Guid ticketId,
        Guid actorId,
        string action,
        string? detail,
        decimal? costAmount,
        string? costNote,
        Guid? vendorId,
        string? chargeTo,
        int? warrantyPeriodDays,
        DateTime? warrantyExpiresAt,
        DateTime createdAt)
        => new(id, ticketId, actorId, action, detail, costAmount, costNote, vendorId, chargeTo, warrantyPeriodDays, warrantyExpiresAt, createdAt);
}
