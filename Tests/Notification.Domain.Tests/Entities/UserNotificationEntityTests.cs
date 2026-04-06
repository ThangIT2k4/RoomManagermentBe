using Notification.Domain.Entities;

namespace Notification.Domain.Tests.Entities;

public sealed class UserNotificationEntityTests
{
    [Fact]
    public void Create_WithValidInput_ShouldCreateUnreadNotification()
    {
        var userId = Guid.NewGuid();

        var entity = UserNotificationEntity.Create(userId, "  Hello  ", "  Welcome  ", "  Info  ");

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.Equal(userId, entity.UserId);
        Assert.Equal("Hello", entity.Title);
        Assert.Equal("Welcome", entity.Content);
        Assert.Equal("Info", entity.Type);
        Assert.False(entity.IsRead);
        Assert.Null(entity.ReadAt);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => UserNotificationEntity.Create(Guid.Empty, "t", "c"));
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => UserNotificationEntity.Create(Guid.NewGuid(), "", "c"));
    }
}

