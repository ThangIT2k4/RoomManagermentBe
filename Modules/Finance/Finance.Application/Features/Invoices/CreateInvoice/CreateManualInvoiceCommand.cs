using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Invoices.CreateInvoice;

public sealed record CreateManualInvoiceCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid LeaseId,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyList<InvoiceItemLineDto> Items)
    : IAppRequest<Result<InvoiceDto>>;
