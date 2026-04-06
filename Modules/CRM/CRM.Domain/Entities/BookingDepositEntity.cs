using CRM.Domain.Common;
using CRM.Domain.Enums;
using CRM.Domain.Exceptions;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Entities;

public sealed class BookingDepositEntity
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? LeadId { get; private set; }
    public Guid? ViewingId { get; private set; }
    public decimal Amount { get; private set; }
    public string? DepositType { get; private set; }
    public string PaymentStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private BookingDepositEntity(
        Guid id,
        Guid organizationId,
        Guid? leadId,
        Guid? viewingId,
        decimal amount,
        string? depositType,
        string paymentStatus,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        LeadId = leadId;
        ViewingId = viewingId;
        Amount = amount;
        DepositType = depositType;
        PaymentStatus = paymentStatus;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static BookingDepositEntity Create(
        Guid organizationId,
        decimal amount,
        Guid? leadId = null,
        Guid? viewingId = null,
        string? depositType = null,
        string paymentStatus = "pending",
        DateTime? createdAt = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new DomainValidationException("OrganizationId is required.");
        }

        return new BookingDepositEntity(
            Guid.NewGuid(),
            organizationId,
            leadId,
            viewingId,
            MoneyAmount.Create(amount, fieldName: nameof(amount)).Value,
            depositType?.Trim(),
            EnumValueParser.ParseRequired<PaymentStatus>(paymentStatus, nameof(paymentStatus)),
            createdAt ?? DateTime.UtcNow,
            null);
    }

    public static BookingDepositEntity Reconstitute(
        Guid id,
        Guid organizationId,
        Guid? leadId,
        Guid? viewingId,
        decimal amount,
        string? depositType,
        string paymentStatus,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new BookingDepositEntity(id, organizationId, leadId, viewingId, amount, depositType, paymentStatus, createdAt, updatedAt);
    }
}
