namespace Finance.Domain.Entities;

public sealed class PaymentEntity
{
    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Guid? MethodId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? ReferenceCode { get; private set; }
    public string Status { get; private set; } = "success";
    public string? RawPayload { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    private PaymentEntity()
    {
    }

    public static PaymentEntity CreateSuccess(
        Guid invoiceId,
        Guid? methodId,
        decimal amount,
        DateTime paidAt,
        string? referenceCode,
        string? rawPayload)
    {
        if (invoiceId == Guid.Empty)
        {
            throw new ArgumentException("InvoiceId is required.", nameof(invoiceId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        var now = DateTime.UtcNow;
        return new PaymentEntity
        {
            Id = Guid.CreateVersion7(),
            InvoiceId = invoiceId,
            MethodId = methodId,
            Amount = amount,
            PaidAt = paidAt,
            ReferenceCode = referenceCode?.Trim(),
            Status = "success",
            RawPayload = rawPayload,
            CreatedAt = now
        };
    }

    public static PaymentEntity FromPersistence(
        Guid id,
        Guid invoiceId,
        Guid? methodId,
        decimal amount,
        DateTime? paidAt,
        string? referenceCode,
        string status,
        string? rawPayload,
        string? errorMessage,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        Guid? deletedBy)
    {
        return new PaymentEntity
        {
            Id = id,
            InvoiceId = invoiceId,
            MethodId = methodId,
            Amount = amount,
            PaidAt = paidAt,
            ReferenceCode = referenceCode,
            Status = status,
            RawPayload = rawPayload,
            ErrorMessage = errorMessage,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            DeletedBy = deletedBy
        };
    }
}
