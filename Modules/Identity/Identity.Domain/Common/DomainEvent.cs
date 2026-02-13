namespace Identity.Domain.Common;

public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent() {}

    protected DomainEvent(DateTimeOffset occurredOn)
    {
        EventId = Guid.NewGuid();
        OccurredOn = occurredOn;
    }
    
    public Guid EventId { get; }
    public DateTimeOffset OccurredOn { get; }
}