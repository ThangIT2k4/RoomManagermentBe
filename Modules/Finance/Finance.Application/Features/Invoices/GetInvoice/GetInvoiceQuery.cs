using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.GetInvoice;

public sealed record GetInvoiceQuery(Guid OrganizationId, Guid InvoiceId)
    : IAppRequest<Result<InvoiceDto>>;

public sealed record GetInvoiceForTenantQuery(Guid TenantUserId, Guid InvoiceId)
    : IAppRequest<Result<InvoiceDto>>;
