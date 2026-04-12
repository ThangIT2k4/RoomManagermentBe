using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.CancelInvoice;

public sealed record CancelInvoiceCommand(Guid OrganizationId, Guid UserId, Guid InvoiceId, string? Reason)
    : IAppRequest<Result<InvoiceDto>>;
