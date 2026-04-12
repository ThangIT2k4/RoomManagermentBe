using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.PublishInvoice;

public sealed record PublishInvoiceCommand(Guid OrganizationId, Guid InvoiceId)
    : IAppRequest<Result<InvoiceDto>>;
