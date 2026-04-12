using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Payments.HandleWebhook;

public sealed record HandlePaymentWebhookCommand(string RawBody, IReadOnlyDictionary<string, string> Headers)
    : IAppRequest<Result>;
