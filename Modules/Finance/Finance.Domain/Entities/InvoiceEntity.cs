using Finance.Domain;

namespace Finance.Domain.Entities;

public sealed class InvoiceEntity
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? LeaseId { get; private set; }
    public bool IsAutoCreated { get; private set; }
    public string? InvoiceNo { get; private set; }
    public DateOnly InvoiceDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public string Status { get; private set; } = InvoiceStatuses.Draft;
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public string? Notes { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    private InvoiceEntity()
    {
    }

    public static InvoiceEntity CreateDraft(
        Guid organizationId,
        Guid? leaseId,
        DateOnly invoiceDate,
        DateOnly dueDate,
        decimal totalAmount,
        string? notes,
        Guid? createdBy,
        bool isAutoCreated,
        string? invoiceNo = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("OrganizationId is required.", nameof(organizationId));
        }

        if (dueDate < invoiceDate)
        {
            throw new ArgumentException("Due date must be on or after invoice date.", nameof(dueDate));
        }

        if (totalAmount <= 0)
        {
            throw new ArgumentException("Total amount must be positive.", nameof(totalAmount));
        }

        var now = DateTime.UtcNow;
        return new InvoiceEntity
        {
            Id = Guid.CreateVersion7(),
            OrganizationId = organizationId,
            LeaseId = leaseId,
            IsAutoCreated = isAutoCreated,
            InvoiceNo = string.IsNullOrWhiteSpace(invoiceNo) ? null : invoiceNo.Trim(),
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            Status = InvoiceStatuses.Draft,
            TotalAmount = totalAmount,
            PaidAmount = 0,
            Notes = notes?.Trim(),
            CreatedBy = createdBy,
            CreatedAt = now,
            UpdatedAt = null
        };
    }

    public static InvoiceEntity FromPersistence(
        Guid id,
        Guid organizationId,
        Guid? leaseId,
        bool isAutoCreated,
        string? invoiceNo,
        DateOnly invoiceDate,
        DateOnly dueDate,
        string status,
        decimal totalAmount,
        decimal paidAmount,
        string? notes,
        Guid? createdBy,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        Guid? deletedBy)
    {
        return new InvoiceEntity
        {
            Id = id,
            OrganizationId = organizationId,
            LeaseId = leaseId,
            IsAutoCreated = isAutoCreated,
            InvoiceNo = invoiceNo,
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            Status = status,
            TotalAmount = totalAmount,
            PaidAmount = paidAmount,
            Notes = notes,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            DeletedBy = deletedBy
        };
    }

    public void SetInvoiceNo(string invoiceNo) => InvoiceNo = invoiceNo.Trim();

    public void Publish()
    {
        if (!InvoiceRules.CanPublish(Status))
        {
            throw new InvalidOperationException("Only draft invoices can be published.");
        }

        Status = InvoiceStatuses.Sent;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (!InvoiceRules.CanCancel(Status, PaidAmount))
        {
            throw new InvalidOperationException("Invoice cannot be cancelled.");
        }

        Status = InvoiceStatuses.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkOverdue()
    {
        if (!Status.Equals(InvoiceStatuses.Sent, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Status = InvoiceStatuses.Overdue;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyPayment(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        if (!InvoiceRules.CanRecordPayment(Status))
        {
            throw new InvalidOperationException("Payments are not allowed for this invoice status.");
        }

        PaidAmount += amount;
        var next = InvoiceRules.NextStatusAfterPayment(TotalAmount, PaidAmount);
        if (next is not null)
        {
            Status = next;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void ReplaceDraftTotals(DateOnly dueDate, string? notes, decimal totalAmount)
    {
        if (!InvoiceRules.CanEdit(Status))
        {
            throw new InvalidOperationException("Only draft invoices can be updated.");
        }

        if (dueDate < InvoiceDate)
        {
            throw new ArgumentException("Due date must be on or after invoice date.", nameof(dueDate));
        }

        if (totalAmount <= 0)
        {
            throw new ArgumentException("Total amount must be positive.", nameof(totalAmount));
        }

        DueDate = dueDate;
        Notes = notes?.Trim();
        TotalAmount = totalAmount;
        UpdatedAt = DateTime.UtcNow;
    }
}
