using Notification.Domain.Entities;

namespace Notification.Domain.Tests.Entities;

public sealed class UserNotificationPreferenceEntityTests
{
    [Fact]
    public void Create_WithValidInput_ShouldInitializeDefaults()
    {
        var userId = Guid.NewGuid();

        var entity = UserNotificationPreferenceEntity.Create(userId, "invoice");

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.Equal(userId, entity.UserId);
        Assert.Equal("invoice", entity.EntityType);
        Assert.True(entity.InAppEnabled);
        Assert.False(entity.EmailEnabled);
        Assert.NotEqual(default, entity.CreatedAt);
        Assert.Null(entity.UpdatedAt);
    }

    [Fact]
    public void Create_WithEmptyEntityType_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => UserNotificationPreferenceEntity.Create(Guid.NewGuid(), ""));
    }
}

