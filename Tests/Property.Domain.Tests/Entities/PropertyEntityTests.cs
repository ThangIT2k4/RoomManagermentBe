// using Property.Domain.Entities;
//
// namespace Property.Domain.Tests.Entities;
//
// public sealed class PropertyEntityTests
// {
//     [Fact]
//     public void Create_WithValidInput_ShouldCreateProperty()
//     {
//         var entity = PropertyEntity.Create(Guid.NewGuid(), "  Tower A  ", 100);
//
//         Assert.Equal("Tower A", entity.Name);
//         Assert.Equal(100, entity.TotalUnits);
//     }
//
//     [Fact]
//     public void Create_WithEmptyName_ShouldThrowArgumentException()
//     {
//         Assert.Throws<ArgumentException>(() => PropertyEntity.Create(Guid.NewGuid(), "", 1));
//     }
// }
//
