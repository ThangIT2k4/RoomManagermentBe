using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Domain.Exceptions;

namespace Auth.Domain.Tests.Entities;

public sealed class UserEntityAuthFlowTests
{
    [Fact]
    public void RegisterByEmail_ShouldCreateInactiveUser()
    {
        var user = UserEntity.RegisterByEmail("user@example.com", "demo", "+84123456789", "PasswordHash01");

        Assert.Equal(UserStatus.Inactive, user.Status);
        Assert.False(user.CanLogin());
    }

    [Fact]
    public void VerifyEmailAndActivate_ShouldActivateInactiveUser()
    {
        var user = UserEntity.RegisterByEmail("user@example.com", "demo", "+84123456789", "PasswordHash01");
        var verifiedAt = DateTime.UtcNow;

        user.VerifyEmailAndActivate(verifiedAt);

        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Equal(verifiedAt, user.EmailVerifiedAt);
        Assert.True(user.CanLogin());
    }

    [Fact]
    public void RecordLogin_ShouldThrow_WhenUserIsBanned()
    {
        var user = UserEntity.Create("user@example.com", "demo");
        user.Ban(DateTime.UtcNow);

        var ex = Assert.Throws<InvalidUserStateException>(() => user.RecordLogin(DateTime.UtcNow));

        Assert.Equal(InvalidUserStateException.CodeBannedUser, ex.ErrorCode);
    }

    [Fact]
    public void RecordLogin_ShouldThrow_WhenUserIsInactive()
    {
        var user = UserEntity.RegisterByEmail("user@example.com", "demo", "+84123456789", "PasswordHash01");

        var ex = Assert.Throws<InvalidUserStateException>(() => user.RecordLogin(DateTime.UtcNow));

        Assert.Equal(InvalidUserStateException.CodeInactiveUser, ex.ErrorCode);
    }

    [Fact]
    public void SoftDelete_ShouldPreventFurtherLogin()
    {
        var user = UserEntity.Create("user@example.com", "demo");
        user.SoftDelete(Guid.NewGuid(), DateTime.UtcNow);

        Assert.False(user.CanLogin());
    }
}
