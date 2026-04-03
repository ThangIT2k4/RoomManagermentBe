using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Tests.Entities;

public sealed class UserEntityTests
{
    [Fact]
    public void Create_ShouldInitializeUserAsInactive_AndRaiseUserCreatedEvent()
    {
        var user = UserEntity.Create(
            Guid.NewGuid(),
            Username.Create("tester01"),
            Email.Create("tester01@example.com"),
            PasswordHash.Create("hash-v1"));

        Assert.Equal(UserStatus.InActive, user.Status);
        Assert.Contains(user.DomainEvents, e => e is UserCreatedEvent);
    }

    [Fact]
    public void Activate_WhenUtcDate_ShouldSetActiveAndRaiseEvent()
    {
        var user = BuildUser();

        user.Activate(DateTime.UtcNow);

        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Contains(user.DomainEvents, e => e is UserActivatedEvent);
    }

    [Fact]
    public void Activate_WhenLocalDate_ShouldThrowArgumentException()
    {
        var user = BuildUser();

        Assert.Throws<ArgumentException>(() => user.Activate(DateTime.Now));
    }

    [Fact]
    public void ChangePassword_WithDifferentHash_ShouldUpdateHashAndRaiseEvent()
    {
        var user = BuildUser();
        var newHash = PasswordHash.Create("hash-v2");

        user.ChangePassword(newHash);

        Assert.Equal("hash-v2", user.PasswordHash.Value);
        Assert.Contains(user.DomainEvents, e => e is UserPasswordChangedEvent);
    }

    [Fact]
    public void ChangePassword_WithSameHash_ShouldThrowInvalidPasswordHashException()
    {
        var user = BuildUser();

        var ex = Assert.Throws<InvalidPasswordHashException>(() => user.ChangePassword(PasswordHash.Create("hash-v1")));

        Assert.Equal(InvalidPasswordHashException.CodeInvalid, ex.ErrorCode);
    }

    private static UserEntity BuildUser() => UserEntity.Create(
        Guid.NewGuid(),
        Username.Create("tester01"),
        Email.Create("tester01@example.com"),
        PasswordHash.Create("hash-v1"));
}

