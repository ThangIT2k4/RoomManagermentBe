// using Lease.Domain.Entities;
//
// namespace Lease.Domain.Tests.Entities;
//
// public sealed class LeaseEntityTests
// {
//     [Fact]
//     public void Create_WithValidInput_ShouldCreateLease()
//     {
//         var entity = LeaseEntity.Create(Guid.NewGuid(), Guid.NewGuid(), 500m, new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
//
//         Assert.Equal(500m, entity.RentAmount);
//         Assert.Equal("draft", entity.Status);
//     }
//
//     [Fact]
//     public void Create_WithEmptyUnitId_ShouldThrowArgumentException()
//     {
//         Assert.Throws<ArgumentException>(() => LeaseEntity.Create(Guid.Empty, Guid.NewGuid(), 1m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 2)));
//     }
// }
//
