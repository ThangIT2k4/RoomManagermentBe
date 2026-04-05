namespace Finance.Domain.Common;

public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTimeOffset.UtcNow;
    }

    protected DomainEvent(DateTimeOffset occurredOn)
    {
        EventId = Guid.NewGuid();
        OccurredOn = occurredOn;
    }

    public Guid EventId { get; }
    public DateTimeOffset OccurredOn { get; }
}
