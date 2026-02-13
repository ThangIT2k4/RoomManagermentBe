namespace Identity.Domain.Common;

public interface IDomainEvent
{
    Guid EventId { get;  }
    DateTimeOffset OccurredOn { get;  }
}
