namespace Property.Domain.Common;

public abstract class Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public TId Id { get; protected set; } = default!;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
