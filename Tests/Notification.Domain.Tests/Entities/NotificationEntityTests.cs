using Notification.Domain.Entities;

namespace Notification.Domain.Tests.Entities;

public sealed class NotificationEntityTests
{
    [Fact]
    public void Create_WithValidInput_ShouldTrimAndDefaultType()
    {
        var userId = Guid.NewGuid();
        var entity = NotificationEntity.Create(userId, "  Hello  ", "  Welcome  ");

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.Equal(userId, entity.UserId);
        Assert.Equal("Hello", entity.Title);
        Assert.Equal("Welcome", entity.Content);
        Assert.Equal("Info", entity.Type);
        Assert.False(entity.IsRead);
        Assert.Null(entity.ReadAt);
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NotificationEntity.Create(Guid.NewGuid(), "", "content"));
    }
}

