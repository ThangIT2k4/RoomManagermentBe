namespace Property.Domain.Entities;

public sealed class MeterReadingEntity
{
    public Guid Id { get; }
    public Guid MeterId { get; }
    public DateOnly ReadingDate { get; }
    public decimal Value { get; }
    public string? ImageUrl { get; }
    public string? Note { get; }
    public Guid? TakenBy { get; }
    public DateTime CreatedAt { get; }

    private MeterReadingEntity(
        Guid id,
        Guid meterId,
        DateOnly readingDate,
        decimal value,
        string? imageUrl,
        string? note,
        Guid? takenBy,
        DateTime createdAt)
    {
        Id = id;
        MeterId = meterId;
        ReadingDate = readingDate;
        Value = value;
        ImageUrl = imageUrl;
        Note = note;
        TakenBy = takenBy;
        CreatedAt = createdAt;
    }

    public static MeterReadingEntity FromPersistence(
        Guid id,
        Guid meterId,
        DateOnly readingDate,
        decimal value,
        string? imageUrl,
        string? note,
        Guid? takenBy,
        DateTime createdAt)
        => new(id, meterId, readingDate, value, imageUrl, note, takenBy, createdAt);
}
