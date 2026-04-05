namespace Auth.Domain.Common;

public abstract record DomainEvent(DateTimeOffset OccurredOn) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
}