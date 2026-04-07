using CRM.Domain.Common;

namespace CRM.Domain.Events;

public sealed record ReviewReplyUpdatedEvent(
    Guid ReplyId,
    Guid ReviewId,
    Guid UserId,
    DateTimeOffset OccurredOn) : DomainEvent;
