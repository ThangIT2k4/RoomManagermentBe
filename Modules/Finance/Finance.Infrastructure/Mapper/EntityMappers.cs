using DalDepositRefund = RoomManagerment.Finance.EntityClasses.DepositRefundEntity;
using DalInvoice = RoomManagerment.Finance.EntityClasses.InvoiceEntity;
using DalInvoiceItem = RoomManagerment.Finance.EntityClasses.InvoiceItemEntity;
using DalPayment = RoomManagerment.Finance.EntityClasses.PaymentEntity;
using DomainDepositRefund = Finance.Domain.Entities.DepositRefundEntity;
using DomainInvoice = Finance.Domain.Entities.InvoiceEntity;
using DomainInvoiceItem = Finance.Domain.Entities.InvoiceItemEntity;
using DomainPayment = Finance.Domain.Entities.PaymentEntity;

namespace Finance.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static DomainInvoice ToDomain(this DalInvoice dal)
    {
        return DomainInvoice.FromPersistence(
            dal.Id,
            dal.OrganizationId,
            dal.LeaseId,
            dal.IsAutoCreated,
            dal.InvoiceNo,
            dal.InvoiceDate,
            dal.DueDate,
            dal.Status ?? Finance.Domain.InvoiceStatuses.Draft,
            dal.TotalAmount,
            dal.PaidAmount,
            dal.Notes,
            dal.CreatedBy,
            dal.CreatedAt,
            dal.UpdatedAt,
            dal.DeletedAt,
            dal.DeletedBy);
    }

    public static DomainInvoiceItem ToDomain(this DalInvoiceItem dal)
    {
        return DomainInvoiceItem.FromPersistence(
            dal.Id,
            dal.InvoiceId,
            dal.ItemType ?? string.Empty,
            dal.Description,
            dal.Quantity,
            dal.UnitPrice,
            dal.Amount,
            dal.ServiceId,
            dal.MeterReadingId,
            dal.TicketLogId,
            dal.CreatedAt,
            dal.UpdatedAt,
            dal.DeletedAt,
            dal.DeletedBy);
    }

    public static DomainPayment ToDomain(this DalPayment dal)
    {
        return DomainPayment.FromPersistence(
            dal.Id,
            dal.InvoiceId,
            dal.MethodId,
            dal.Amount,
            dal.PaidAt,
            dal.ReferenceCode,
            dal.Status ?? "pending",
            dal.RawPayload,
            dal.ErrorMessage,
            dal.CreatedAt,
            dal.UpdatedAt,
            dal.DeletedAt,
            dal.DeletedBy);
    }

    public static DomainDepositRefund ToDomain(this DalDepositRefund dal)
    {
        return DomainDepositRefund.FromPersistence(
            dal.Id,
            dal.LeaseId,
            dal.OrganizationId,
            dal.TenantId,
            dal.Amount,
            dal.Status ?? "pending",
            dal.Notes,
            dal.CreatedBy,
            dal.CreatedAt,
            dal.UpdatedAt,
            dal.PaidAt,
            dal.PaidBy,
            dal.DeletedAt,
            dal.DeletedBy);
    }
}
