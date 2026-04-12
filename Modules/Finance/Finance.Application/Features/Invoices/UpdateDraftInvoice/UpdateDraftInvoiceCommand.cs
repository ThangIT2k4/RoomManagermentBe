using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.UpdateDraftInvoice;

public sealed record UpdateDraftInvoiceCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid InvoiceId,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyList<InvoiceItemLineDto> Items)
    : IAppRequest<Result<InvoiceDto>>;
