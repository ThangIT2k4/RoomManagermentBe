using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Payments.RecordPayment;

public sealed record RecordManualPaymentCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid InvoiceId,
    Guid MethodId,
    decimal Amount,
    DateTime PaidAtUtc,
    string? ReferenceCode,
    string? Note)
    : IAppRequest<Result<InvoiceDto>>;
