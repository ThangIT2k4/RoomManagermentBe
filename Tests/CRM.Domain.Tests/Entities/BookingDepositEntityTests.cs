using CRM.Domain.Entities;
using CRM.Domain.Events;

namespace CRM.Domain.Tests.Entities;

public sealed class BookingDepositEntityTests
{
    [Fact]
    public void Approve_ShouldMoveStatusToApproved()
    {
        var entity = BookingDepositEntity.Create(Guid.NewGuid(), 1000m);
        entity.ClearDomainEvents();

        entity.Approve();

        Assert.Equal("approved", entity.PaymentStatus);
        Assert.Contains(entity.DomainEvents, x => x is BookingDepositStatusChangedEvent);
    }

    [Fact]
    public void MarkPaid_ShouldMoveStatusToPaid()
    {
        var entity = BookingDepositEntity.Create(Guid.NewGuid(), 1000m);
        entity.MarkPaid();

        Assert.Equal("paid", entity.PaymentStatus);
    }
}
