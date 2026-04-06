using Notification.Infrastructure.Mapper;

namespace Notification.Infrastructure.Tests.Mapper;

public sealed class EntityMappersTests
{
    [Fact]
    public void ToDomain_ShouldMapNotificationFields()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 4, 5, 12, 30, 0, DateTimeKind.Utc);
        var dal = new RoomManagerment.Notification.EntityClasses.NotificationEntity
        {
            Id = id,
            UserId = userId,
            Title = "  New notification  ",
            Message = "  Body text  ",
            Type = "  Info  ",
            CreatedAt = createdAt,
            IsRead = false,
            ReadAt = null
        };

        var domain = dal.ToDomain();

        Assert.Equal(id, domain.Id);
        Assert.Equal(userId, domain.UserId);
        Assert.Equal(Guid.Empty, domain.NotificationChannelId);
        Assert.Equal("  New notification  ", domain.Title);
        Assert.Equal("  Body text  ", domain.Content);
        Assert.Equal("  Info  ", domain.Type);
        Assert.Equal(createdAt, domain.CreatedAt);
        Assert.False(domain.IsRead);
        Assert.Null(domain.ReadAt);
    }

    [Fact]
    public void ToUserNotificationDomain_ShouldProjectCurrentNotificationRow()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 4, 5, 13, 15, 0, DateTimeKind.Utc);
        var dal = new RoomManagerment.Notification.EntityClasses.NotificationEntity
        {
            Id = id,
            UserId = userId,
            NotificationChannelId = Guid.NewGuid(),
            Title = "Hello",
            Message = "Welcome",
            Type = "Info",
            CreatedAt = createdAt,
            IsRead = true,
            ReadAt = createdAt.AddMinutes(5)
        };

        var domain = dal.ToUserNotificationDomain();

        Assert.Equal(id, domain.Id);
        Assert.Equal(userId, domain.UserId);
        Assert.Equal("Hello", domain.Title);
        Assert.Equal("Welcome", domain.Content);
        Assert.Equal("Info", domain.Type);
        Assert.Equal(createdAt, domain.CreatedAt);
        Assert.True(domain.IsRead);
        Assert.Equal(createdAt.AddMinutes(5), domain.ReadAt);
    }

    [Fact]
    public void Preference_ToDomain_ShouldMapAllFields()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 4, 5, 14, 0, 0, DateTimeKind.Utc);
        var updatedAt = createdAt.AddMinutes(10);

        var dal = new RoomManagerment.Notification.EntityClasses.UserNotificationPreferenceEntity
        {
            Id = id,
            UserId = userId,
            EntityType = "invoice",
            InAppEnabled = true,
            EmailEnabled = false,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var domain = dal.ToDomain();

        Assert.Equal(id, domain.Id);
        Assert.Equal(userId, domain.UserId);
        Assert.Equal("invoice", domain.EntityType);
        Assert.True(domain.InAppEnabled);
        Assert.False(domain.EmailEnabled);
        Assert.Equal(createdAt, domain.CreatedAt);
        Assert.Equal(updatedAt, domain.UpdatedAt);
    }
}


