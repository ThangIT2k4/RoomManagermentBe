using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Jobs.RunAutoInvoice;

public sealed record RunAutoInvoiceCommand(DateOnly RunDate) : IAppRequest<Result<int>>;
