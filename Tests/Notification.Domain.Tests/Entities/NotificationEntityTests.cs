using Notification.Domain.Entities;

namespace Notification.Domain.Tests.Entities;

public sealed class NotificationEntityTests
{
    [Fact]
    public void Create_WithValidInput_ShouldTrimAndDefaultType()
    {
        var entity = NotificationEntity.Create("  Hello  ", "  Welcome  ");

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.Equal("Hello", entity.Title);
        Assert.Equal("Welcome", entity.Content);
        Assert.Equal("Info", entity.Type);
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NotificationEntity.Create("", "content"));
    }
}

