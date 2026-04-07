using Lease.Domain.Entities;

namespace Lease.Domain.Tests.Entities;

public sealed class LeaseEntityTests
{
    [Fact]
    public void Create_WithValidInput_ShouldCreateActiveLease()
    {
        var entity = LeaseEntity.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            500m,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            paymentDay: 5);

        Assert.Equal(500m, entity.RentAmount);
        Assert.Equal(LeaseStatuses.Active, entity.Status);
        Assert.Equal(5, entity.PaymentDay);
    }

    [Fact]
    public void Create_WithInvalidEndDate_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            LeaseEntity.Create(Guid.NewGuid(), Guid.NewGuid(), 1m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 1)));
    }

    [Fact]
    public void Terminate_ShouldSetStatusAndEndDate()
    {
        var lease = LeaseEntity.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1000m,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31));

        lease.Terminate(new DateOnly(2026, 10, 1), "Tenant request", DateTime.UtcNow);

        Assert.Equal(LeaseStatuses.Terminated, lease.Status);
        Assert.Equal(new DateOnly(2026, 10, 1), lease.EndDate);
    }
}
