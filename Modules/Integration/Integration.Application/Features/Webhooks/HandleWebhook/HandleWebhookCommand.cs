using Integration.Application.Common;
using Integration.Domain.Enums;
using MediatR;

namespace Integration.Application.Features.Webhooks.HandleWebhook;

public sealed class HandleWebhookCommand : IRequest<Result>
{
    public required IntegrationProvider Provider { get; init; }
    public required string Signature { get; init; }
    public required string PayloadJson { get; init; }
    public required IReadOnlyDictionary<string, string> Headers { get; init; }
}
