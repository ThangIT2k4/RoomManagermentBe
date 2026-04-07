using Organization.Domain.Entities;

namespace Organization.Domain.Tests.Entities;

public sealed class OrganizationEntityTests
{
    [Fact]
    public void Create_ShouldRaiseCreatedEvent()
    {
        var entity = OrganizationEntity.Create("Acme");
        Assert.NotEmpty(entity.DomainEvents);
    }

    [Fact]
    public void UpdateProfile_ShouldChangeFields()
    {
        var entity = OrganizationEntity.Create("Acme");
        entity.UpdateProfile("Acme 2", "0901", "a@b.com", null, "123", "HN", DateTime.UtcNow);
        Assert.Equal("Acme 2", entity.Name);
        Assert.Equal("0901", entity.Phone);
    }
}
// using Organization.Domain.Entities;
//
// namespace Organization.Domain.Tests.Entities;
//
// public sealed class OrganizationEntityTests
// {
//     [Fact]
//     public void Create_WithValidInput_ShouldCreateOrganization()
//     {
//         var entity = OrganizationEntity.Create("  ACME  ");
//
//         Assert.Equal("ACME", entity.Name);
//         Assert.Equal((short)1, entity.Status);
//     }
//
//     [Fact]
//     public void Create_WithEmptyName_ShouldThrowArgumentException()
//     {
//         Assert.Throws<ArgumentException>(() => OrganizationEntity.Create(""));
//     }
// }

