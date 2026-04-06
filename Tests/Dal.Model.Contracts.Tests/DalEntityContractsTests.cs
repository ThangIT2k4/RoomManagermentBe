namespace Dal.Model.Contracts.Tests;

public sealed class DalEntityContractsTests
{
    [Fact]
    public void Auth_DalContracts_ShouldExposeExpectedEntities()
    {
        var id = Guid.NewGuid();
        var user = new RoomManagerment.Auth.EntityClasses.UserEntity { Id = id };
        var queryFactory = new RoomManagerment.Auth.FactoryClasses.QueryFactory();

        Assert.Equal(id, user.Id);
        Assert.NotNull(queryFactory.User);
        Assert.True(Enum.IsDefined(typeof(RoomManagerment.Auth.EntityType), "UserEntity"));
    }

    [Fact]
    public void Crm_DalContracts_ShouldExposeExpectedEntities()
    {
        var id = Guid.NewGuid();
        var lead = new RoomManagerment.CRM.EntityClasses.LeadEntity { Id = id };
        var queryFactory = new RoomManagerment.CRM.FactoryClasses.QueryFactory();

        Assert.Equal(id, lead.Id);
        Assert.NotNull(queryFactory.Lead);
        Assert.True(Enum.IsDefined(typeof(RoomManagerment.CRM.EntityType), "LeadEntity"));
    }

    [Fact]
    public void Finance_DalContracts_ShouldExposeExpectedEntities()
    {
        var id = Guid.NewGuid();
        var invoice = new RoomManagerment.Finance.EntityClasses.InvoiceEntity { Id = id };
        var queryFactory = new RoomManagerment.Finance.FactoryClasses.QueryFactory();

        Assert.Equal(id, invoice.Id);
        Assert.NotNull(queryFactory.Invoice);
        Assert.True(Enum.IsDefined(typeof(RoomManagerment.Finance.EntityType), "InvoiceEntity"));
    }

    [Fact]
    public void Lease_DalContracts_ShouldExposeExpectedEntities()
    {
        var id = Guid.NewGuid();
        var lease = new RoomManagerment.Lease.EntityClasses.LeaseEntity { Id = id };
        var queryFactory = new RoomManagerment.Lease.FactoryClasses.QueryFactory();

        Assert.Equal(id, lease.Id);
        Assert.NotNull(queryFactory.Lease);
        Assert.True(Enum.IsDefined(typeof(RoomManagerment.Lease.EntityType), "LeaseEntity"));
    }

    [Fact]
    public void Organization_DalContracts_ShouldExposeExpectedEntities()
    {
        var id = Guid.NewGuid();
        var organization = new RoomManagerment.Organizations.EntityClasses.OrganizationEntity { Id = id };
        var queryFactory = new RoomManagerment.Organizations.FactoryClasses.QueryFactory();

        Assert.Equal(id, organization.Id);
        Assert.NotNull(queryFactory.Organization);
        Assert.True(Enum.IsDefined(typeof(RoomManagerment.Organizations.EntityType), "OrganizationEntity"));
    }

    [Fact]
    public void Property_DalContracts_ShouldExposeExpectedEntities()
    {
        var id = Guid.NewGuid();
        var property = new RoomManagerment.Property.EntityClasses.PropertyEntity { Id = id };
        var queryFactory = new RoomManagerment.Property.FactoryClasses.QueryFactory();

        Assert.Equal(id, property.Id);
        Assert.NotNull(queryFactory.Property);
        Assert.True(Enum.IsDefined(typeof(RoomManagerment.Property.EntityType), "PropertyEntity"));
    }

    [Fact]
    public void Notification_DalContracts_ShouldUseNewEntityModel()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var notification = new RoomManagerment.Notification.EntityClasses.NotificationEntity
        {
            Id = id,
            UserId = userId
        };
        var queryFactory = new RoomManagerment.Notification.FactoryClasses.QueryFactory();

        Assert.Equal(id, notification.Id);
        Assert.Equal(userId, notification.UserId);
        Assert.NotNull(queryFactory.Notification);
        Assert.NotNull(queryFactory.UserNotificationPreference);
        Assert.True(Enum.IsDefined(typeof(RoomManagerment.Notification.EntityType), "UserNotificationPreferenceEntity"));
        Assert.False(Enum.IsDefined(typeof(RoomManagerment.Notification.EntityType), "UserNotificationEntity"));
    }
}

