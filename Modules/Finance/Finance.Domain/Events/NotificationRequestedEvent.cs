using Finance.Domain.Common;

namespace Finance.Domain.Events;

public sealed record NotificationRequestedEvent(
    Guid RecipientUserId,
    string Title,
    string Message,
    string Type = "Info",
    IReadOnlyDictionary<string, string>? Metadata = null) : DomainEvent();
