using CRM.Domain.Entities;
using CRM.Domain.Events;
using CRM.Domain.Exceptions;

namespace CRM.Domain.Tests.Entities;

public sealed class ViewingEntityTests
{
    [Fact]
    public void Confirm_FromScheduled_ShouldSetConfirmedAndRaiseEvent()
    {
        var entity = ViewingEntity.Create(Guid.NewGuid(), DateTime.UtcNow.AddHours(1));
        entity.ClearDomainEvents();

        entity.Confirm();

        Assert.Equal("confirmed", entity.Status);
        Assert.Contains(entity.DomainEvents, x => x is ViewingStatusChangedEvent);
    }

    [Fact]
    public void Complete_FromCancelled_ShouldThrow()
    {
        var entity = ViewingEntity.Create(Guid.NewGuid(), DateTime.UtcNow.AddHours(1));
        entity.Cancel("test");

        Assert.Throws<DomainValidationException>(() => entity.Complete("done"));
    }
}
