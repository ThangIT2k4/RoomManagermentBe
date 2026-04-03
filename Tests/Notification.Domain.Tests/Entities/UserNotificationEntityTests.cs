using Notification.Domain.Entities;

namespace Notification.Domain.Tests.Entities;

public sealed class UserNotificationEntityTests
{
    [Fact]
    public void Create_WithValidIds_ShouldCreateUnreadNotification()
    {
        var userId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        var entity = UserNotificationEntity.Create(userId, notificationId);

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.Equal(userId, entity.UserId);
        Assert.Equal(notificationId, entity.NotificationId);
        Assert.False(entity.IsRead);
        Assert.Null(entity.ReadAt);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => UserNotificationEntity.Create(Guid.Empty, Guid.NewGuid()));
    }
}

