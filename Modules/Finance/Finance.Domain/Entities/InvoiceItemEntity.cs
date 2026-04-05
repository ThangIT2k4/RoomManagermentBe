namespace Finance.Domain.Entities;

public sealed class InvoiceItemEntity
{
    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public string ItemType { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Amount { get; private set; }
    public Guid? ServiceId { get; private set; }
    public Guid? MeterReadingId { get; private set; }
    public Guid? TicketLogId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    private InvoiceItemEntity()
    {
    }

    public static InvoiceItemEntity Create(
        Guid invoiceId,
        string itemType,
        string? description,
        decimal quantity,
        decimal unitPrice,
        Guid? serviceId,
        Guid? meterReadingId,
        Guid? ticketLogId)
    {
        if (invoiceId == Guid.Empty)
        {
            throw new ArgumentException("InvoiceId is required.", nameof(invoiceId));
        }

        if (string.IsNullOrWhiteSpace(itemType))
        {
            throw new ArgumentException("Item type is required.", nameof(itemType));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        }

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice));
        }

        var amount = Math.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero);
        var now = DateTime.UtcNow;
        return new InvoiceItemEntity
        {
            Id = Guid.CreateVersion7(),
            InvoiceId = invoiceId,
            ItemType = itemType.Trim(),
            Description = description?.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            Amount = amount,
            ServiceId = serviceId,
            MeterReadingId = meterReadingId,
            TicketLogId = ticketLogId,
            CreatedAt = now
        };
    }

    public static InvoiceItemEntity FromPersistence(
        Guid id,
        Guid invoiceId,
        string itemType,
        string? description,
        decimal quantity,
        decimal unitPrice,
        decimal amount,
        Guid? serviceId,
        Guid? meterReadingId,
        Guid? ticketLogId,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        Guid? deletedBy)
    {
        return new InvoiceItemEntity
        {
            Id = id,
            InvoiceId = invoiceId,
            ItemType = itemType,
            Description = description,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Amount = amount,
            ServiceId = serviceId,
            MeterReadingId = meterReadingId,
            TicketLogId = ticketLogId,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            DeletedBy = deletedBy
        };
    }

    public void SoftDelete(Guid deletedBy, DateTime utcNow)
    {
        DeletedAt = utcNow;
        DeletedBy = deletedBy;
        UpdatedAt = utcNow;
    }
}
