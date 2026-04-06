namespace CRM.Domain.Common;

public interface IDomainEvent
{
    Guid EventId { get;  }
    DateTimeOffset OccurredOn { get;  }
}
